using AutoFixture;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Report.Invoices;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Report.Invoices;

[TestFixture]
public class InvoicesReportHandlerTests : BaseTestFixture
{
    private Mock<IReportManager> _reportManagerMock;
    private InvoicesReportHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _reportManagerMock = new Mock<IReportManager>();
        _handler = new InvoicesReportHandler(_reportManagerMock.Object);
    }

    [Test]
    public async Task InvoicesReport_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var dto = Fixture.Create<ExportReportDTO>();
        var expectedResult = new ExportResult
        {
            FileName = $"InvoicesReport_{DateTime.UtcNow:yyyy-MM-dd}.csv",
            ContentType = "text/csv",
            Stream = new MemoryStream()
        };

        _reportManagerMock
            .Setup(x => x.GenerateInvoicesReportAsync(dto, CancellationToken.None))
            .ReturnsAsync(expectedResult);

        var request = new InvoicesReportRequest(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.FileName, Is.EqualTo(expectedResult.FileName));
        Assert.That(result.Value.ContentType, Is.EqualTo(expectedResult.ContentType));
        Assert.That(result.Value.Stream, Is.EqualTo(expectedResult.Stream));

        _reportManagerMock.Verify(
            x => x.GenerateInvoicesReportAsync(
                It.Is<ExportReportDTO>(r => r == dto),
                CancellationToken.None),
            Times.Once);
    }
}
