using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Plan.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Plan.GetById;

[TestFixture]
public class GetPlanByIdHandlerTests : BaseTestFixture
{
    private Mock<IPlanRepository> _planRepositoryMock;

    private GetPlanByIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _handler = new GetPlanByIdHandler(_planRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetPlanById_ValidRequest_PlanReturned()
    {
        // Arrange
        var plan = Fixture.Create<Data.Models.Plan>();

        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(plan.Id, CancellationToken.None))
            .ReturnsAsync(plan);

        // Act
        var request = new GetPlanByIdRequest(plan.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);

        Assert.That(result.Value.Id, Is.EqualTo(plan.Id));
        Assert.That(result.Value.ClientId, Is.EqualTo(plan.ClientId));
        Assert.That(result.Value.ItemId, Is.EqualTo(plan.ItemId));
        Assert.That(result.Value.StartDate, Is.EqualTo(plan.StartDate));
        Assert.That(result.Value.EndDate, Is.EqualTo(plan.EndDate));
        Assert.That(result.Value.Count, Is.EqualTo(plan.Count));

        _planRepositoryMock.Verify(
            x => x.GetByIdAsync(plan.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetPlanById_PlanNotFound_ReturnsNotFound()
    {
        // Arrange
        var plan = Fixture.Create<Data.Models.Plan>();

        // Act
        var request = new GetPlanByIdRequest(plan.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _planRepositoryMock.Verify(
            repo => repo.GetByIdAsync(plan.Id, CancellationToken.None),
            Times.Once);
    }
}
