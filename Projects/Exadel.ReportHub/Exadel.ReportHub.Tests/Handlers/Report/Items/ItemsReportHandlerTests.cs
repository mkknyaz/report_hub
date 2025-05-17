using AutoFixture;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Report.Items;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Report.Items;

[TestFixture]
public class ItemsReportHandlerTests : BaseTestFixture
{
    private Mock<IReportManager> _reportManagerMock;
    private ItemsReportHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _reportManagerMock = new Mock<IReportManager>();
        _handler = new ItemsReportHandler(_reportManagerMock.Object);
    }

    [Test]
    public async Task ItemsReport_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var dto = Fixture.Create<ExportReportDTO>();

        var expectedResult = new ExportResult
        {
            FileName = $"ItemsReport_{DateTime.UtcNow:yyyy-MM-dd}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Stream = new MemoryStream()
        };

        _reportManagerMock
            .Setup(x => x.GenerateItemsReportAsync(dto, CancellationToken.None))
            .ReturnsAsync(expectedResult);

        var request = new ItemsReportRequest(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.FileName, Is.EqualTo(expectedResult.FileName));
        Assert.That(result.Value.ContentType, Is.EqualTo(expectedResult.ContentType));
        Assert.That(result.Value.Stream, Is.EqualTo(expectedResult.Stream));

        _reportManagerMock.Verify(
            x => x.GenerateItemsReportAsync(
                It.Is<ExportReportDTO>(r => r == dto),
                CancellationToken.None),
            Times.Once);
    }
}
