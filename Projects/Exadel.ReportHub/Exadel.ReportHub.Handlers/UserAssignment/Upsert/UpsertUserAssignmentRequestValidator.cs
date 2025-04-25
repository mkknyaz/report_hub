using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.UserAssignment.Upsert;

public class UpsertUserAssignmentRequestValidator : AbstractValidator<UpsertUserAssignmentRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;

    public UpsertUserAssignmentRequestValidator(IUserRepository userRepository, IClientRepository clientRepository)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.UpsertUserAssignmentDto)
            .ChildRules(child =>
            {
                child.RuleLevelCascadeMode = CascadeMode.Stop;
                child.ClassLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.UserId)
                    .NotEmpty()
                    .MustAsync(_userRepository.ExistsAsync)
                    .WithMessage(Constants.Validation.User.DoesNotExist);

                child.RuleFor(x => x.ClientId)
                    .NotEmpty()
                    .MustAsync(_clientRepository.ExistsAsync)
                    .WithMessage(Constants.Validation.Client.DoesNotExist);

                child.RuleFor(x => x.Role)
                    .IsInEnum();

                child.When(x => x.Role == UserRole.SuperAdmin, () =>
                {
                    child.RuleFor(x => x.ClientId)
                        .Must(id => id == Constants.ClientData.GlobalId)
                        .WithMessage(Constants.Validation.UserAssignment.GlobalRoleAssignment);
                }).Otherwise(() =>
                {
                    child.RuleFor(x => x.ClientId)
                        .Must(id => id != Constants.ClientData.GlobalId)
                        .WithMessage(Constants.Validation.UserAssignment.ClientRoleAssignment);
                });
            });
    }
}
