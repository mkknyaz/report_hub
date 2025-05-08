using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdateNotificationFrequency;
public class UpdateUserNotificationSettingsRequestValidator : AbstractValidator<UpdateUserNotificationSettingsRequest>
{
    public UpdateUserNotificationSettingsRequestValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.NotificationSettingsDTO)
            .ChildRules(child =>
            {
                child.ClassLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.Hour)
                   .InclusiveBetween(0, Constants.Validation.NotificationSettings.MaxHour)
                   .WithMessage(Constants.Validation.NotificationSettings.TimeHourRange);

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
                        .WithMessage(Constants.Validation.NotificationSettings.ShouldBeSet);
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
                        .InclusiveBetween(1, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month))
                        .WithMessage(Constants.Validation.NotificationSettings.MonthDayRange);
                });

                child.RuleFor(x => x.ReportStartDate)
                  .NotEmpty()
                  .LessThan(x => x.ReportEndDate)
                  .WithMessage(Constants.Validation.Date.InvalidStartDate);

                child.RuleFor(x => x.ReportEndDate)
                  .NotEmpty()
                  .LessThan(DateTime.UtcNow)
                  .WithMessage(Constants.Validation.Date.EndDateNotInPast);
            });
    }
}
