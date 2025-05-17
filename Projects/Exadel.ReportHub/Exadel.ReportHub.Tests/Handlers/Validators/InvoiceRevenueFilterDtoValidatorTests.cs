using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class InvoiceRevenueFilterDtoValidatorTests : BaseTestFixture
{
    private InvoiceRevenueFilterDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new InvoiceRevenueFilterDtoValidator();
    }

    [Test]
    public async Task ValidateAsync_EmptyStartDate_ErrorReturned()
    {
        // Arrange
        var dto = GetValidInvoiceRevenue();
        dto.StartDate = DateTime.MinValue.Date;

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(InvoiceRevenueFilterDTO.StartDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.EmptyStartDate));
    }

    [Test]
    public async Task ValidateAsync_StartDateAfterEndDate_ErrorReturned()
    {
        // Arrange
        var dto = GetValidInvoiceRevenue();
        dto.StartDate = dto.EndDate.AddDays(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(InvoiceRevenueFilterDTO.StartDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.InvalidStartDate));
    }

    [Test]
    public async Task ValidateAsync_EndDateInFuture_ErrorReturned()
    {
        // Arrange
        var dto = GetValidInvoiceRevenue();
        dto.EndDate = DateTime.UtcNow.Date.AddDays(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(InvoiceRevenueFilterDTO.EndDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.EndDateNotInPast));
    }

    [Test]
    public async Task ValidateAsync_StartDateWithTimeComponent_ErrorReturned()
    {
        // Arrange
        var dto = GetValidInvoiceRevenue();
        dto.StartDate = dto.StartDate.AddSeconds(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("StartDate.TimeOfDay"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.TimeComponentNotAllowed));
    }

    [Test]
    public async Task ValidateAsync_EndDateWithTimeComponent_ErrorReturned()
    {
        // Arrange
        var dto = GetValidInvoiceRevenue();
        dto.EndDate = dto.EndDate.AddSeconds(1);

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("EndDate.TimeOfDay"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.TimeComponentNotAllowed));
    }

    [Test]
    public async Task ValidateAsync_ValidDates_NoErrorReturned()
    {
        // Arrange
        var dto = Fixture.Build<InvoiceRevenueFilterDTO>()
            .With(x => x.StartDate, DateTime.UtcNow.Date.AddDays(-2))
            .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(-1))
            .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    private InvoiceRevenueFilterDTO GetValidInvoiceRevenue()
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-7);
        var endDate = DateTime.UtcNow.Date.AddDays(-5);

        return Fixture.Build<InvoiceRevenueFilterDTO>()
            .With(x => x.StartDate, startDate)
            .With(x => x.EndDate, endDate)
            .Create();
    }
}
