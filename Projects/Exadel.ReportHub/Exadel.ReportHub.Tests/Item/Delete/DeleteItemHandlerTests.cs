using ErrorOr;
using Exadel.ReportHub.Handlers.Item.Delete;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Item.Delete;

[TestFixture]
public class DeleteItemHandlerTests
{
    private Mock<IItemRepository> _itemRepositoryMock;
    private DeleteItemHandler _handler;

    [SetUp]
    public void Setup()
    {
        _itemRepositoryMock = new Mock<IItemRepository>();
        _handler = new DeleteItemHandler(_itemRepositoryMock.Object);
    }

    [Test]
    public async Task DeleteItem_ItemExists_ReturnsDeleted()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _itemRepositoryMock
            .Setup(x => x.ExistsAsync(itemId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new DeleteItemRequest(itemId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Deleted));

        _itemRepositoryMock.Verify(
            x => x.ExistsAsync(itemId, CancellationToken.None),
            Times.Once);

        _itemRepositoryMock.Verify(
            x => x.SoftDeleteAsync(itemId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeleteItem_ItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _itemRepositoryMock
            .Setup(x => x.ExistsAsync(itemId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new DeleteItemRequest(itemId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _itemRepositoryMock.Verify(
            x => x.ExistsAsync(itemId, CancellationToken.None),
            Times.Once);

        _itemRepositoryMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
