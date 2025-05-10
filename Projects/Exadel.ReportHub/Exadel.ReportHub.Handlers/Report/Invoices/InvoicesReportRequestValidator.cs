using Exadel.ReportHub.SDK.DTOs.Report;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Report.Invoices;

public class InvoicesReportRequestValidator : AbstractValidator<InvoicesReportRequest>
{
    private readonly IValidator<ExportReportDTO> _exportReportValidator;

    public InvoicesReportRequestValidator(IValidator<ExportReportDTO> exportReportValidator)
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
