using Exadel.ReportHub.SDK.DTOs.Report;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Report.Plans;

public class PlansReportRequestValidator : AbstractValidator<PlansReportRequest>
{
    private readonly IValidator<ExportReportDTO> _exportReportValidator;

    public PlansReportRequestValidator(IValidator<ExportReportDTO> exportReportValidator)
    {
        _exportReportValidator = exportReportValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.ExportReportDto)
            .SetValidator(_exportReportValidator);
    }
}
