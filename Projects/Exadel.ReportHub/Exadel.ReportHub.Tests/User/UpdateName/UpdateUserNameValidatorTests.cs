using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.User.UpdateName;
using Exadel.ReportHub.Handlers.Validators;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.User.UpdateName;

public class UpdateUserNameValidatorTests
{
    private UpdateUserNameRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        var userNameValidator = new StringValidator();

        _validator = new UpdateUserNameRequestValidator(userNameValidator);
    }

    [Test]
    public async Task ValidateAsync_FullNameIsEmpty_ErrorReturned()
    {
        var userId = Guid.NewGuid();
        var createUserRequest = new UpdateUserNameRequest(userId, string.Empty);
        var result = await _validator.TestValidateAsync(createUserRequest);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage($"'{nameof(Constants.Validation.Name)}' must not be empty.");
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task ValidateAsync_FullNameExceedsMaxLength_ErrorReturned()
    {
        var userId = Guid.NewGuid();
        var maxLength = Constants.Validation.Name.MaxLength + 1;
        var fullname = new string('x', maxLength);
        var createUserRequest = new UpdateUserNameRequest(userId, fullname);
        var result = await _validator.TestValidateAsync(createUserRequest);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage($"The length of '{nameof(Constants.Validation.Name)}' must be 100 characters or fewer. You entered {maxLength} characters.");
        Assert.That(result.Errors, Has.Count.EqualTo(1));
    }
}
