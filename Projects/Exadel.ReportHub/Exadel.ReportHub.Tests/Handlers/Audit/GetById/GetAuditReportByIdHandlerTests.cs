using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Audit.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Audit.GetById;

[TestFixture]
public class GetAuditReportByIdHandlerTests : BaseTestFixture
{
    private Mock<IAuditReportRepository> _auditReportRepositoryMock;

    private GetAuditReportByIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _auditReportRepositoryMock = new Mock<IAuditReportRepository>();
        _handler = new GetAuditReportByIdHandler(_auditReportRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetAuditReportById_ValidRequest_AuditReportReturned()
    {
        // Arrange
        var report = Fixture.Create<Data.Models.AuditReport>();

        _auditReportRepositoryMock
            .Setup(x => x.GetByIdAsync(report.Id, CancellationToken.None))
            .ReturnsAsync(report);

        // Act
        var request = new GetAuditReportByIdRequest(report.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);

        Assert.That(result.Value.Id, Is.EqualTo(report.Id));
        Assert.That(result.Value.Properties, Is.EqualTo(report.Properties));
        Assert.That(result.Value.TimeStamp, Is.EqualTo(report.TimeStamp));
        Assert.That(result.Value.Action, Is.EqualTo(report.Action));
        Assert.That(result.Value.IsSuccess, Is.EqualTo(report.IsSuccess));

        _auditReportRepositoryMock.Verify(
            x => x.GetByIdAsync(report.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetAuditReportById_AuditReportNotFound_ReturnsNotFound()
    {
        // Arrange
        var report = Fixture.Create<Data.Models.AuditReport>();

        // Act
        var request = new GetAuditReportByIdRequest(report.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _auditReportRepositoryMock.Verify(
            repo => repo.GetByIdAsync(report.Id, CancellationToken.None),
            Times.Once);
    }
}
