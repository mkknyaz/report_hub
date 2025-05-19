using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class ExportReportDtoValidatorTests : BaseTestFixture
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private ExportReportDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _validator = new ExportReportDtoValidator(_clientRepositoryMock.Object);
    }

    [Test]
    public async Task ValidateAsync_ValidExportReportDTO_NoErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task ValidateAsync_EmptyClientId_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.ClientId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(dto.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ClientDoesNotExist_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.ClientId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(dto.ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(dto.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_StartDateAfterEndDate_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.StartDate = DateTime.UtcNow;
        dto.EndDate = dto.StartDate.Value.AddDays(-1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("StartDate.Value"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.InvalidStartDate));
    }

    [Test]
    public async Task ValidateAsync_StartDateInFutureWithoutEndDate_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.StartDate = DateTime.UtcNow.AddDays(1);
        dto.EndDate = null;

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("StartDate.Value"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.InFuture));
    }

    [Test]
    public async Task ValidateAsync_StartDateHasTime_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.StartDate = dto.StartDate.Value.AddSeconds(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("StartDate.Value.TimeOfDay"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.TimeComponentNotAllowed));
    }

    [Test]
    public async Task ValidateAsync_EndDateInFuture_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.EndDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("EndDate.Value"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.InFuture));
    }

    [Test]
    public async Task ValidateAsync_EndDateHasTime_ErrorReturned()
    {
        // Arrange
        var dto = SetupValidReport();
        dto.EndDate = DateTime.UtcNow.Date.AddHours(5);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("EndDate.Value.TimeOfDay"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.TimeComponentNotAllowed));
    }

    private ExportReportDTO SetupValidReport()
    {
        var clientId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        return Fixture.Build<ExportReportDTO>()
            .With(x => x.ClientId, clientId)
            .With(x => x.StartDate, DateTime.UtcNow.Date.AddDays(-5))
            .With(x => x.EndDate, DateTime.UtcNow.Date)
            .Create();
    }
}
