using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdateRole;

internal class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.Role)
            .IsInEnum();
    }
}
