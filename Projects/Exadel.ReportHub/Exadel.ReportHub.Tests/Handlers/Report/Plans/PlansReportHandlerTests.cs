using AutoFixture;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Notifications.Report.Plan;
using Exadel.ReportHub.Handlers.Report.Plans;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.Tests.Abstracts;
using MediatR;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Report.Plans;

[TestFixture]
public class PlansReportHandlerTests : BaseTestFixture
{
    private Mock<IReportManager> _reportManagerMock;
    private Mock<IUserProvider> _userProviderMock;
    private Mock<IPublisher> _publisherMock;
    private PlansReportHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _reportManagerMock = new Mock<IReportManager>();
        _userProviderMock = new Mock<IUserProvider>();
        _publisherMock = new Mock<IPublisher>();
        _handler = new PlansReportHandler(
            _reportManagerMock.Object,
            _userProviderMock.Object,
            _publisherMock.Object);
    }

    [Test]
    public async Task PlansReport_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var dto = Fixture.Create<ExportReportDTO>();
        var userId = Guid.NewGuid();
        var expectedResult = new ExportResult
        {
            FileName = $"PlansReport_{DateTime.UtcNow:yyyy-MM-dd}.csv",
            ContentType = "text/csv",
            Stream = new MemoryStream()
        };
        _userProviderMock
            .Setup(x => x.GetUserId())
            .Returns(userId);
        _reportManagerMock
            .Setup(x => x.GeneratePlansReportAsync(dto, CancellationToken.None))
            .ReturnsAsync(expectedResult);

        var request = new PlansReportRequest(dto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.FileName, Is.EqualTo(expectedResult.FileName));
        Assert.That(result.Value.ContentType, Is.EqualTo(expectedResult.ContentType));
        Assert.That(result.Value.Stream, Is.EqualTo(expectedResult.Stream));

        _reportManagerMock.Verify(
            x => x.GeneratePlansReportAsync(
                It.Is<ExportReportDTO>(r => r == dto),
                CancellationToken.None),
            Times.Once);

        _publisherMock.Verify(
            x => x.Publish(It.Is<PlansReportExportedNotification>(
                    x => x.UserId == userId &&
                         x.ClientId == dto.ClientId &&
                         x.IsSuccess),
                CancellationToken.None),
            Times.Once);
    }
}
