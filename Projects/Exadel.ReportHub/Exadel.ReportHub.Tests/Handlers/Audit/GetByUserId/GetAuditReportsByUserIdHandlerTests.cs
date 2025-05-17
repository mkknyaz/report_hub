using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Audit.GetByUserId;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Pagination;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Audit.GetByUserId;

[TestFixture]
public class GetAuditReportsByUserIdHandlerTests : BaseTestFixture
{
    private Mock<IAuditReportRepository> _auditReportRepositoryMock;

    private GetAuditReportsByUserIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _auditReportRepositoryMock = new Mock<IAuditReportRepository>();
        _handler = new GetAuditReportsByUserIdHandler(_auditReportRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetAuditReports_UserHasAuditReports_ReturnsPageRequestDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var auditReports = Fixture.Build<Data.Models.AuditReport>().With(x => x.UserId, userId).CreateMany(2).ToList();
        var pageRequestDto = Fixture.Create<PageRequestDTO>();

        _auditReportRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, pageRequestDto.Top, CancellationToken.None))
            .ReturnsAsync(auditReports);

        // Act
        var request = new GetAuditReportsByUserIdRequest(userId, pageRequestDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Items.ToList(), Has.Count.EqualTo(auditReports.Count));

        for (int i = 0; i < result.Value.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Value.Items[i].Id, Is.EqualTo(auditReports[i].Id));
                Assert.That(result.Value.Items[i].UserId, Is.EqualTo(auditReports[i].UserId));
                Assert.That(result.Value.Items[i].Properties, Is.EqualTo(auditReports[i].Properties));
                Assert.That(result.Value.Items[i].TimeStamp, Is.EqualTo(auditReports[i].TimeStamp));
                Assert.That(result.Value.Items[i].Action, Is.EqualTo(auditReports[i].Action));
                Assert.That(result.Value.Items[i].IsSuccess, Is.EqualTo(auditReports[i].IsSuccess));
            });
        }

        _auditReportRepositoryMock.Verify(
            x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, pageRequestDto.Top, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetAuditReports_UserHasNoAuditReports_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pageRequestDto = Fixture.Create<PageRequestDTO>();

        _auditReportRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, pageRequestDto.Top, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.AuditReport>());

        // Act
        var request = new GetAuditReportsByUserIdRequest(userId, pageRequestDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Items, Is.Empty);

        _auditReportRepositoryMock.Verify(
            x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, pageRequestDto.Top, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetAuditReports_TopIsZero_DefaultPageSizeApplied()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var auditReports = Fixture.Build<Data.Models.AuditReport>()
            .With(x => x.UserId, userId)
            .CreateMany(Constants.Validation.Pagination.MaxValue)
            .ToList();

        var pageRequestDto = new PageRequestDTO
        {
            Top = 0,
            Skip = 0
        };

        _auditReportRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, Constants.Validation.Pagination.MaxValue, CancellationToken.None))
            .ReturnsAsync(auditReports);

        _auditReportRepositoryMock
            .Setup(x => x.GetCountAsync(userId, CancellationToken.None))
            .ReturnsAsync(Constants.Validation.Pagination.MaxValue);

        // Act
        var request = new GetAuditReportsByUserIdRequest(userId, pageRequestDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Items.Count, Is.EqualTo(Constants.Validation.Pagination.MaxValue));
        Assert.That(result.Value.Count, Is.EqualTo(Constants.Validation.Pagination.MaxValue));

        _auditReportRepositoryMock.Verify(
            x => x.GetByUserIdAsync(userId, pageRequestDto.Skip, Constants.Validation.Pagination.MaxValue, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetAuditReports_TopIsNonZero_ReturnsExpectedReports()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var top = 5;
        var skip = 0;
        var auditReports = Fixture.Build<Data.Models.AuditReport>()
            .With(x => x.UserId, userId)
            .CreateMany(top).ToList();

        var pageRequestDto = new PageRequestDTO
        {
            Top = top,
            Skip = skip
        };

        _auditReportRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, skip, top, CancellationToken.None))
            .ReturnsAsync(auditReports);

        // Act
        var request = new GetAuditReportsByUserIdRequest(userId, pageRequestDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Items, Has.Count.EqualTo(top));
    }
}
