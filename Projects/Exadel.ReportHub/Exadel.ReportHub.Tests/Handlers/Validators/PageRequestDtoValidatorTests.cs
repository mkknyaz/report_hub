using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.SDK.DTOs.Pagination;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class PageRequestDtoValidatorTests : BaseTestFixture
{
    private PageRequestDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new PageRequestDtoValidator();
    }

    [Test]
    public async Task ValidateAsync_TopNegative_ErrorReturned()
    {
        // Arrange
        var dto = Fixture.Build<PageRequestDTO>()
            .With(x => x.Top, -1)
            .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(PageRequestDTO.Top)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Top' must be greater than or equal to '0'."));
    }

    [Test]
    public async Task ValidateAsync_TopExceedsMaxValue_ErrorReturned()
    {
        // Arrange
        var dto = Fixture.Build<PageRequestDTO>()
            .With(x => x.Top, Constants.Validation.Pagination.MaxValue)
            .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(PageRequestDTO.Top)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'Top' must be less than '{Constants.Validation.Pagination.MaxValue}'."));
    }

    [Test]
    public async Task ValidateAsync_SkipNegative_ErrorReturned()
    {
        // Arrange
        var expectedPage = new PageRequestDTO
        {
            Top = Constants.Validation.Pagination.MaxValue - 10,
            Skip = -1
        };

        // Act
        var result = await _validator.TestValidateAsync(expectedPage);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(PageRequestDTO.Skip)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Skip' must be greater than or equal to '0'."));
    }

    [Test]
    public async Task ValidateAsync_ValidValues_NoErrorReturned()
    {
        // Arrange
        var expectedPage = new PageRequestDTO
        {
            Top = Constants.Validation.Pagination.MaxValue - 10,
            Skip = 0
        };

        // Act
        var result = await _validator.TestValidateAsync(expectedPage);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }
}
