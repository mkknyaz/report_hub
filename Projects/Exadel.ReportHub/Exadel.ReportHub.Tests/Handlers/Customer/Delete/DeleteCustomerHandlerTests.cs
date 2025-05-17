using ErrorOr;
using Exadel.ReportHub.Handlers.Customer.Delete;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Customer.Delete;

[TestFixture]
public class DeleteCustomerHandlerTests
{
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private DeleteCustomerHandler _handler;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new DeleteCustomerHandler(_customerRepositoryMock.Object);
    }

    [Test]
    public async Task DeleteCustomer_CustomerExists_ReturnsDeleted()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _customerRepositoryMock
            .Setup(x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new DeleteCustomerRequest(customerId, clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Deleted));

        _customerRepositoryMock.Verify(
            x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.SoftDeleteAsync(customerId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeleteCustomer_CustomerDoesNotExist_ReturnsNotFound()
    {// Arrange
        var customerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _customerRepositoryMock
            .Setup(x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new DeleteCustomerRequest(customerId, clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _customerRepositoryMock.Verify(
            x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
