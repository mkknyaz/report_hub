using Exadel.ReportHub.SDK.DTOs.Pagination;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Audit.GetByUserId;

public class GetAuditReportsByUserIdValidator : AbstractValidator<GetAuditReportsByUserIdRequest>
{
    private readonly IValidator<PageRequestDTO> _pageRequestValidator;

    public GetAuditReportsByUserIdValidator(IValidator<PageRequestDTO> pageRequestValidator)
    {
        _pageRequestValidator = pageRequestValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.PageRequestDto)
            .SetValidator(_pageRequestValidator);
    }
}
