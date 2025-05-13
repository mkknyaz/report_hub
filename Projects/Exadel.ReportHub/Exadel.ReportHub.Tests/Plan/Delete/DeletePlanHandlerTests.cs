using ErrorOr;
using Exadel.ReportHub.Handlers.Plan.Delete;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Plan.Delete;

[TestFixture]
public class DeletePlanHandlerTests
{
    private Mock<IPlanRepository> _planRepositoryMock;

    private DeletePlanHandler _handler;

    [SetUp]
    public void Setup()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _handler = new DeletePlanHandler(_planRepositoryMock.Object);
    }

    [Test]
    public async Task DeletePlan_PlanExists_ReturnsDeleted()
    {
        // Arrange
        var planId = Guid.NewGuid();

        _planRepositoryMock
            .Setup(x => x.ExistsAsync(planId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new DeletePlanRequest(planId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Deleted));

        _planRepositoryMock.Verify(
            x => x.ExistsAsync(planId, CancellationToken.None),
            Times.Once);

        _planRepositoryMock.Verify(
            x => x.SoftDeleteAsync(planId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeletePlan_PlanDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var planId = Guid.NewGuid();

        _planRepositoryMock
            .Setup(x => x.ExistsAsync(planId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new DeletePlanRequest(planId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _planRepositoryMock.Verify(
            x => x.ExistsAsync(planId, CancellationToken.None),
            Times.Once);

        _planRepositoryMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Guid>(), CancellationToken.None),
            Times.Never);
    }
}
