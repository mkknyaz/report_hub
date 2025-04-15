using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.User.Create;
using Exadel.ReportHub.Handlers.User.UpdateName;
using Exadel.ReportHub.SDK.DTOs.User;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.User.UpdateName;

public class UpdateUserNameValidatorTests
{
    private UpdateUserNameRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        var userNameValidator = new InlineValidator<string>();
        userNameValidator.RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(Constants.Validation.User.FullNameMaxLength)
            .WithName(nameof(CreateUserDTO.FullName));

        _validator = new UpdateUserNameRequestValidator(userNameValidator);
    }

    [Test]
    public async Task ValidateAsync_FullNameIsEmpty_ErrorReturned()
    {
        var userId = Guid.NewGuid();
        var createUserRequest = new UpdateUserNameRequest(userId, string.Empty);
        var result = await _validator.TestValidateAsync(createUserRequest);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("'FullName' must not be empty.");
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task ValidateAsync_FullNameExceedsMaxLength_ErrorReturned()
    {
        var userId = Guid.NewGuid();
        var maxLength = 101;
        var fullname = new string('x', maxLength);
        var createUserRequest = new UpdateUserNameRequest(userId, fullname);
        var result = await _validator.TestValidateAsync(createUserRequest);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage($"The length of 'FullName' must be 100 characters or fewer. You entered {maxLength} characters.");
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }
}
