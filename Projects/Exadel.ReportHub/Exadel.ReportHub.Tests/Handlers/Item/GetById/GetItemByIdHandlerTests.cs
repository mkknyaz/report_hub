using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Item.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Item.GetById;

[TestFixture]
public class GetItemByIdHandlerTests : BaseTestFixture
{
    private Mock<IItemRepository> _itemRepositoryMock;
    private GetItemByIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _handler = new GetItemByIdHandler(_itemRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task Handle_ItemExists_ReturnsItemDTO()
    {
        // Arrange
        var item = Fixture.Create<Data.Models.Item>();

        _itemRepositoryMock
            .Setup(x => x.GetByIdAsync(item.Id, CancellationToken.None))
            .ReturnsAsync(item);

        // Act
        var request = new GetItemByIdRequest(item.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);

        Assert.That(result.Value.Id, Is.EqualTo(item.Id));
        Assert.That(result.Value.ClientId, Is.EqualTo(item.ClientId));
        Assert.That(result.Value.Name, Is.EqualTo(item.Name));
        Assert.That(result.Value.Description, Is.EqualTo(item.Description));
        Assert.That(result.Value.Price, Is.EqualTo(item.Price));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(item.CurrencyId));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(item.CurrencyCode));

        _itemRepositoryMock.Verify(
            x => x.GetByIdAsync(item.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task Handle_ItemNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _itemRepositoryMock
            .Setup(x => x.GetByIdAsync(itemId, CancellationToken.None))
            .ReturnsAsync((Data.Models.Item)null);

        // Act
        var request = new GetItemByIdRequest(itemId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _itemRepositoryMock.Verify(
            x => x.GetByIdAsync(itemId, CancellationToken.None),
            Times.Once);
    }
}
