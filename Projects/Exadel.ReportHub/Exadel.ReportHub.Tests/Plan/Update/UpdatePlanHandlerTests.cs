using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Plan.Update;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Plan.Update;

[TestFixture]
public class UpdatePlanHandlerTests : BaseTestFixture
{
    private Mock<IPlanRepository> _planRepositoryMock;

    private UpdatePlanHandler _handler;

    [SetUp]
    public void Setup()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _handler = new UpdatePlanHandler(_planRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task UpdatePlan_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var updateDto = Fixture.Create<UpdatePlanDTO>();

        _planRepositoryMock
            .Setup(x => x.ExistsAsync(planId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdatePlanRequest(planId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _planRepositoryMock.Verify(
            x => x.UpdateAsync(
                planId,
                It.Is<Data.Models.Plan>(x =>
                    x.Count == updateDto.Count &&
                    x.StartDate == updateDto.StartDate &&
                    x.EndDate == updateDto.EndDate),
                CancellationToken.None),
            Times.Once);

        _planRepositoryMock.Verify(
            x => x.ExistsAsync(planId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdatePlan_PlanNotFound_ReturnsNotFound()
    {
        // Arrange
        var plan = Fixture.Create<Data.Models.Plan>();
        var updateDto = Fixture.Create<UpdatePlanDTO>();

        _planRepositoryMock
            .Setup(x => x.ExistsAsync(plan.Id, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdatePlanRequest(plan.Id, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _planRepositoryMock.Verify(
            x => x.ExistsAsync(plan.Id, CancellationToken.None),
            Times.Once);

        _planRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Data.Models.Plan>(), CancellationToken.None),
            Times.Never);
    }
}
