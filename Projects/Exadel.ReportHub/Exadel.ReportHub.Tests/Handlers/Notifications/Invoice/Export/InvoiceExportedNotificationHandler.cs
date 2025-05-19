using AutoFixture;
using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Handlers.Notifications.Invoice.Export;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Notifications.Invoice.Export;

[TestFixture]
public class InvoiceExportedNotificationHandler : BaseTestFixture
{
    private Mock<IAuditManager> _auditManagerMock;
    private AuditInvoiceExportedNotificationHandler _handler;

    [SetUp]
    public void Setup()
    {
        _auditManagerMock = new Mock<IAuditManager>();
        _handler = new AuditInvoiceExportedNotificationHandler(_auditManagerMock.Object);
    }

    [Test]
    public async Task Handle_ValidNotification_CallsAuditManager()
    {
        // Arrange
        var notification = Fixture.Build<InvoiceExportedNotification>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.TimeStamp, DateTime.UtcNow)
            .With(x => x.IsSuccess, true)
            .Create();

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _auditManagerMock.Verify(x => x.AuditAsync(
            It.Is<ExportInvoicesAuditAction>(a =>
                a.UserId == notification.UserId &&
                a.TimeStamp == notification.TimeStamp &&
                a.IsSuccess == notification.IsSuccess),
            CancellationToken.None),
            Times.Once);

        var auditAction = _auditManagerMock.Invocations
            .First(i => i.Method.Name == nameof(IAuditManager.AuditAsync))
            .Arguments[0] as ExportInvoicesAuditAction;

        Assert.That(auditAction, Is.Not.Null);
        Assert.That(auditAction.UserId, Is.EqualTo(notification.UserId));
        Assert.That(auditAction.TimeStamp, Is.EqualTo(notification.TimeStamp));
        Assert.That(auditAction.IsSuccess, Is.True);
    }
}
