using Exadel.ReportHub.Common.Authorization;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdateNotificationSettings;

public class UpdateUserNotificationSettingsRequestValidator : AbstractValidator<UpdateUserNotificationSettingsRequest>
{
    private readonly IUserAssignmentRepository _userAssignmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUserProvider _userProvider;

    public UpdateUserNotificationSettingsRequestValidator(IUserAssignmentRepository userAssignmentRepository, IClientRepository clientRepository, IUserProvider userProvider)
    {
        _userAssignmentRepository = userAssignmentRepository;
        _clientRepository = clientRepository;
        _userProvider = userProvider;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.UpdateNotificationSettingsDto)
            .ChildRules(child =>
            {
                child.ClassLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.ClientId)
                    .NotEmpty()
                    .MustAsync(_clientRepository.ExistsAsync)
                    .WithMessage(Constants.Validation.Client.DoesNotExist)
                    .MustAsync(HavePermissionAsync)
                    .WithMessage(Constants.Validation.NotificationSettings.NoPermissions);

                child.RuleFor(x => x.Hour)
                    .InclusiveBetween(0, Constants.Validation.NotificationSettings.MaxHour)
                    .WithMessage(Constants.Validation.NotificationSettings.TimeHourRange);

                child.RuleFor(x => x.ReportPeriod)
                    .IsInEnum();

                child.When(x => x.Frequency == NotificationFrequency.Daily, () =>
                {
                    child.RuleFor(x => x.DayOfWeek)
                        .Null()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldNotBeSet);

                    child.RuleFor(x => x.DayOfMonth)
                        .Null()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldNotBeSet);
                });

                child.When(x => x.Frequency == NotificationFrequency.Weekly, () =>
                {
                    child.RuleFor(x => x.DayOfWeek)
                        .NotNull()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldBeSet)
                        .IsInEnum()
                        .WithMessage(Constants.Validation.NotificationSettings.WeekDayRange);

                    child.RuleFor(x => x.DayOfMonth)
                        .Null()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldNotBeSet);
                });

                child.When(x => x.Frequency == NotificationFrequency.Monthly, () =>
                {
                    child.RuleFor(x => x.DayOfWeek)
                        .Null()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldNotBeSet);

                    child.RuleFor(x => x.DayOfMonth)
                        .NotNull()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldBeSet)
                        .InclusiveBetween(1, Constants.Validation.NotificationSettings.MaxDayOfMonth)
                        .WithMessage(Constants.Validation.NotificationSettings.MonthDayRange);
                });

                child.When(x => x.ReportPeriod is ReportPeriod.Custom, () =>
                {
                    child.RuleFor(x => x.DaysCount)
                        .NotNull()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldBeSet)
                        .GreaterThan(0)
                        .WithMessage(Constants.Validation.NotificationSettings.ZeroDaysCount);
                })
                .Otherwise(() =>
                {
                    child.RuleFor(x => x.DaysCount)
                        .Null()
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldNotBeSet);
                });
            })
            .When(x => x.UpdateNotificationSettingsDto is not null);
    }

    private async Task<bool> HavePermissionAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var role = await _userAssignmentRepository.GetUserRoleByClientIdAsync(_userProvider.GetUserId(), clientId, cancellationToken);
        var allowedRoles = Permissions.GetAllowedRoles(Common.Constants.Authorization.Resource.Reports, Permission.Export);

        return role.HasValue && allowedRoles.Contains(role.Value);
    }
}
