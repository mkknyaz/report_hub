using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ImportDtoValidator : AbstractValidator<ImportDTO>
{
    public ImportDtoValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x)
                .NotNull();

        RuleFor(x => x.File)
                .NotNull()
                .ChildRules(file =>
                {
                    file.RuleFor(x => x.Length)
                        .GreaterThan(0)
                        .WithMessage(Constants.Validation.Import.EmptyFileUpload);

                    file.RuleFor(x => x.FileName)
                        .NotEmpty();
                });
    }
}
