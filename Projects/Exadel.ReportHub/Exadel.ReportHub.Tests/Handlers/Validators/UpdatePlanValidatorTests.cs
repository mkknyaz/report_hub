using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.SDK.DTOs.Plan;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class UpdatePlanValidatorTests : BaseTestFixture
{
    private UpdatePlanValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new UpdatePlanValidator();
    }

    [Test]
    public async Task ValidateAsync_ValidPlan_NoErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task ValidateAsync_CountIsLessThanZero_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();
        plan.Count = -123;

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.Count)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Count' must be greater than '0'."));
    }

    [Test]
    public async Task ValidateAsync_EmptyStartDate_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();
        plan.StartDate = DateTime.MinValue;

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.StartDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Start Date' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_StartDateGreaterThanEndDate_ErrorReturned()
    {
        // Arrange
        var client = SetupValidPlan();
        client.StartDate = client.EndDate.AddDays(1);

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.StartDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.InvalidStartDate));
    }

    [Test]
    public async Task ValidateAsync_EndDateLessThanCurrentDate_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();
        plan.EndDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.EndDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Date.EndDateInPast));
    }

    private UpdatePlanDTO SetupValidPlan()
    {
        return Fixture.Build<UpdatePlanDTO>()
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(-1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(7))
            .Create();
    }
}
