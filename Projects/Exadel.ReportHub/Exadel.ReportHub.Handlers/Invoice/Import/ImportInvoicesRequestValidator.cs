using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.Import;

public class ImportInvoicesRequestValidator : AbstractValidator<ImportInvoicesRequest>
{
    public ImportInvoicesRequestValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.ImportDTO)
                .NotNull();

        RuleFor(x => x.ImportDTO.File)
                .NotNull()
                .ChildRules(file =>
                {
                    file.RuleFor(x => x.Length)
                        .GreaterThan(0)
                        .WithMessage(Constants.Validation.Import.EmptyFileUpload);

                    file.RuleFor(x => x.FileName)
                        .NotEmpty()
                        .Must(fileName => string.Equals(Path.GetExtension(fileName), ".csv", StringComparison.OrdinalIgnoreCase))
                        .WithMessage(Constants.Validation.Import.InvalidFileExtension);
                });
    }
}
