using Exadel.ReportHub.SDK.DTOs.Report;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Report.Items;

public class ItemsReportRequestValidator : AbstractValidator<ItemsReportRequest>
{
    private readonly IValidator<ExportReportDTO> _exportReportValidator;

    public ItemsReportRequestValidator(IValidator<ExportReportDTO> exportReportValidator)
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
