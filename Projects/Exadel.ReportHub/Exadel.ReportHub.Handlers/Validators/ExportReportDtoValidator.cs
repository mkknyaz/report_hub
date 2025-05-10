using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ExportReportDtoValidator : AbstractValidator<ExportReportDTO>
{
    private readonly IClientRepository _clientRepository;

    public ExportReportDtoValidator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x)
            .ChildRules(child =>
            {
                child.RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.ClientId)
                    .NotEmpty()
                    .MustAsync(_clientRepository.ExistsAsync)
                    .WithMessage(Constants.Validation.Client.DoesNotExist);

                child.RuleFor(x => x.Format)
                    .IsInEnum();

                child.When(x => x.StartDate.HasValue, () =>
                {
                    child.When(x => x.EndDate.HasValue, () =>
                    {
                        child.RuleFor(x => x.StartDate.Value)
                            .LessThanOrEqualTo(x => x.EndDate.Value)
                            .WithMessage(Constants.Validation.Date.InvalidStartDate);
                    })
                    .Otherwise(() =>
                    {
                        child.RuleFor(x => x.StartDate.Value)
                            .LessThanOrEqualTo(DateTime.UtcNow)
                            .WithMessage(Constants.Validation.Date.InFuture);
                    });

                    child.RuleFor(x => x.StartDate.Value.TimeOfDay)
                        .Equal(TimeSpan.Zero)
                        .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);
                });

                child.When(x => x.EndDate.HasValue, () =>
                {
                    child.RuleFor(x => x.EndDate.Value)
                        .LessThanOrEqualTo(DateTime.UtcNow)
                        .WithMessage(Constants.Validation.Date.InFuture);

                    child.RuleFor(x => x.EndDate.Value.TimeOfDay)
                        .Equal(TimeSpan.Zero)
                        .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);
                });
            });
    }
}
