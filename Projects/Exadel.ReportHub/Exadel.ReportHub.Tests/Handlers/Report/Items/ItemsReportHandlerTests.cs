using AutoFixture;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Notifications.Report.Item;
using Exadel.ReportHub.Handlers.Report.Items;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.Tests.Abstracts;
using MediatR;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Report.Items;

[TestFixture]
public class ItemsReportHandlerTests : BaseTestFixture
{
    private Mock<IReportManager> _reportManagerMock;
    private Mock<IUserProvider> _userProviderMock;
    private Mock<IPublisher> _publisherMock;
    private ItemsReportHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _reportManagerMock = new Mock<IReportManager>();
        _userProviderMock = new Mock<IUserProvider>();
        _publisherMock = new Mock<IPublisher>();
        _handler = new ItemsReportHandler(
            _reportManagerMock.Object,
            _userProviderMock.Object,
            _publisherMock.Object);
    }

    [Test]
    public async Task ItemsReport_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var dto = Fixture.Create<ExportReportDTO>();
        var userId = Guid.NewGuid();
        var expectedResult = new ExportResult
        {
            FileName = $"ItemsReport_{DateTime.UtcNow:yyyy-MM-dd}.xlsx",
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Stream = new MemoryStream()
        };
        _userProviderMock
            .Setup(x => x.GetUserId())
            .Returns(userId);
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

        _publisherMock.Verify(
            x => x.Publish(It.Is<ItemsReportExportedNotification>(
                    x => x.UserId == userId &&
                         x.ClientId == dto.ClientId &&
                         x.IsSuccess),
                CancellationToken.None),
            Times.Once);
    }
}
