using Exadel.ReportHub.SDK.DTOs.Pagination;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;
public class PageRequestDtoValidator : AbstractValidator<PageRequestDTO>
{
    public PageRequestDtoValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Top)
            .GreaterThanOrEqualTo(0)
            .LessThan(Constants.Validation.Pagination.MaxValue);

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
