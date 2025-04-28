using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Handlers.Item.Update;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Item.Update;

[TestFixture]
public class UpdateItemHandlerTests : BaseTestFixture
{
    private Mock<IItemRepository> _itemRepositoryMock;
    private Mock<ICurrencyRepository> _currencyRepositoryMock;
    private UpdateItemHandler _handler;

    [SetUp]
    public void Setup()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _handler = new UpdateItemHandler(
            _itemRepositoryMock.Object,
            _currencyRepositoryMock.Object,
            Mapper);
    }

    [Test]
    public async Task Handle_ValidUpdate_ReturnsUpdated()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var updateDto = Fixture.Build<CreateUpdateItemDTO>().With(x => x.ClientId, clientId).Create();

        _itemRepositoryMock
            .Setup(x => x.GetClientIdAsync(itemId, CancellationToken.None))
            .ReturnsAsync(clientId);

        _currencyRepositoryMock
            .Setup(x => x.GetCodeByIdAsync(updateDto.CurrencyId, CancellationToken.None))
            .ReturnsAsync(Constants.Currency.DefaultCurrencyCode);

        // Act
        var request = new UpdateItemRequest(itemId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _itemRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Data.Models.Item>(i =>
                    i.Id == itemId &&
                    i.ClientId == clientId &&
                    i.Name == updateDto.Name &&
                    i.Description == updateDto.Description &&
                    i.Price == updateDto.Price &&
                    i.CurrencyId == updateDto.CurrencyId &&
                    i.CurrencyCode == Constants.Currency.DefaultCurrencyCode),
                CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task Handle_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = Fixture.Create<CreateUpdateItemDTO>();

        _itemRepositoryMock
            .Setup(x => x.GetClientIdAsync(itemId, CancellationToken.None))
            .ReturnsAsync((Guid?)null);

        // Act
        var request = new UpdateItemRequest(itemId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _itemRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Data.Models.Item>(), CancellationToken.None),
            Times.Never);
    }

    [Test]
    public async Task Handle_ClientIdChanged_ReturnsValidationError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var originalClientId = Guid.NewGuid();
        var newClientId = Guid.NewGuid();

        var updateDto = Fixture.Build<CreateUpdateItemDTO>().With(x => x.ClientId, newClientId).Create();

        _itemRepositoryMock
            .Setup(x => x.GetClientIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalClientId);

        var request = new UpdateItemRequest(itemId, updateDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.Validation));
        Assert.That(result.FirstError.Description, Is.EqualTo(Handlers.Constants.Validation.Item.ClientIdImmutable));

        _itemRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Data.Models.Item>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
