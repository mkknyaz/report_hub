using AutoFixture;
using Exadel.ReportHub.Handlers.Plan.Create;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Plan.Create;

[TestFixture]
public class CreatePlanHandlerTests : BaseTestFixture
{
    private Mock<IPlanRepository> _planRepositoryMock;

    private CreatePlanHandler _handler;

    [SetUp]
    public void Setup()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _handler = new CreatePlanHandler(_planRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task CreatePlan_ValidRequest_ReturnsPlansDto()
    {
        // Arrange
        var createPlanDto = Fixture.Create<CreatePlanDTO>();

        // Act
        var request = new CreatePlanRequest(createPlanDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<PlanDTO>(), "Returned object should be an instance of CustomerDTO");
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Value.ClientId, Is.EqualTo(createPlanDto.ClientId));
        Assert.That(result.Value.ItemId, Is.EqualTo(createPlanDto.ItemId));
        Assert.That(result.Value.StartDate, Is.EqualTo(createPlanDto.StartDate));
        Assert.That(result.Value.EndDate, Is.EqualTo(createPlanDto.EndDate));
        Assert.That(result.Value.Count, Is.EqualTo(createPlanDto.Count));

        _planRepositoryMock.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.Plan>(
                    c => c.ClientId == createPlanDto.ClientId &&
                    c.ItemId == createPlanDto.ItemId &&
                    c.StartDate == createPlanDto.StartDate &&
                    c.EndDate == createPlanDto.EndDate &&
                    c.Count == createPlanDto.Count),
                CancellationToken.None),
            Times.Once);
    }
}
