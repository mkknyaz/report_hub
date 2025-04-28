using AutoFixture;
using Exadel.ReportHub.Handlers.Item.GetByClientId;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Item.GetByClientId;

[TestFixture]
public class GetItemsByClientIdHandlerTests : BaseTestFixture
{
    private Mock<IItemRepository> _itemRepositoryMock;
    private GetItemsByClientIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _handler = new GetItemsByClientIdHandler(_itemRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task Handle_ClientHasItems_ReturnsItemDTOs()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var items = Fixture.Build<Data.Models.Item>().With(x => x.ClientId, clientId).CreateMany(2).ToList();

        _itemRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync(items);

        // Act
        var request = new GetItemsByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value, Has.Exactly(2).Items);

        for (int i = 0; i < result.Value.Count; i++)
        {
            var expectedItem = new ItemDTO
            {
                Id = items[i].Id,
                ClientId = clientId,
                Name = items[i].Name,
                Description = items[i].Description,
                Price = items[i].Price,
                CurrencyId = items[i].CurrencyId,
                CurrencyCode = items[i].CurrencyCode,
            };
            AssertItemIsCorrect(result.Value[i], expectedItem);
        }

        _itemRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task Handle_ClientHasNoItems_ReturnsEmptyList()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        _itemRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.Item>());

        // Act
        var request = new GetItemsByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Empty);

        _itemRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, CancellationToken.None),
            Times.Once);
    }

    private void AssertItemIsCorrect(ItemDTO item, ItemDTO expectedItem)
    {
        Assert.That(item.Id, Is.EqualTo(expectedItem.Id));
        Assert.That(item.ClientId, Is.EqualTo(expectedItem.ClientId));
        Assert.That(item.Name, Is.EqualTo(expectedItem.Name));
        Assert.That(item.Description, Is.EqualTo(expectedItem.Description));
        Assert.That(item.Price, Is.EqualTo(expectedItem.Price));
        Assert.That(item.CurrencyId, Is.EqualTo(expectedItem.CurrencyId));
        Assert.That(item.CurrencyCode, Is.EqualTo(expectedItem.CurrencyCode));
    }
}
