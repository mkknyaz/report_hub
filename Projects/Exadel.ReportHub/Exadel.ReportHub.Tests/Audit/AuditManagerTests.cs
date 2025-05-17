using AutoFixture;
using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Audit;

[TestFixture]
public class AuditManagerTests : BaseTestFixture
{
    private Mock<IAuditReportRepository> _auditReportRepositoryMock;
    private AuditManager _auditManager;

    [SetUp]
    public void SetUp()
    {
        _auditReportRepositoryMock = new Mock<IAuditReportRepository>();
        _auditManager = new AuditManager(_auditReportRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task Audit_ValidAuditAction_CallsAddAsyncWithMappedAuditReport()
    {
        // Arrange
        var auditAction = Fixture.Create<ExportInvoicesAuditAction>();

        // Act
        await _auditManager.AuditAsync(auditAction, CancellationToken.None);

        // Assert
        _auditReportRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<AuditReport>(ar =>
                    ar.UserId == auditAction.UserId &&
                    ar.TimeStamp == auditAction.TimeStamp &&
                    ar.Action == auditAction.Action &&
                    ar.IsSuccess == auditAction.IsSuccess &&
                    ar.Properties.Count == auditAction.Properties.Count &&
                    ar.Properties["InvoiceId"] == auditAction.Properties["InvoiceId"]),
                CancellationToken.None),
            Times.Once);
    }
}
