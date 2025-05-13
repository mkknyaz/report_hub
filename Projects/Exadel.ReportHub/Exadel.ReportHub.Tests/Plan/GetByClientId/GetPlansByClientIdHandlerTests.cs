using AutoFixture;
using Exadel.ReportHub.Handlers.Plan.GetByClientId;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Plan.GetByClientId;

[TestFixture]
public class GetPlansByClientIdHandlerTests : BaseTestFixture
{
    private Mock<IPlanRepository> _planRepositoryMock;

    private GetPlansByClientIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _handler = new GetPlansByClientIdHandler(_planRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetPlans_ClientHasPlans_ReturnsPlanDTOs()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var plans = Fixture.Build<Data.Models.Plan>().With(x => x.ClientId, clientId).CreateMany(2).ToList();

        _planRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, null, null, CancellationToken.None))
            .ReturnsAsync(plans);

        // Act
        var request = new GetPlansByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(plans.Count));

        for (int i = 0; i < result.Value.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Value[i].Id, Is.EqualTo(plans[i].Id));
                Assert.That(result.Value[i].ClientId, Is.EqualTo(plans[i].ClientId));
                Assert.That(result.Value[i].ItemId, Is.EqualTo(plans[i].ItemId));
                Assert.That(result.Value[i].EndDate, Is.EqualTo(plans[i].EndDate));
                Assert.That(result.Value[i].Count, Is.EqualTo(plans[i].Count));
            });
        }

        _planRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, null, null, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetPlans_ClientHasNoPlans_ReturnsEmptyList()
    {
        var clientId = Guid.NewGuid();

        _planRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, null, null, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.Plan>());

        // Act
        var request = new GetPlansByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Empty);

        _planRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, null, null, CancellationToken.None),
            Times.Once);
    }
}
