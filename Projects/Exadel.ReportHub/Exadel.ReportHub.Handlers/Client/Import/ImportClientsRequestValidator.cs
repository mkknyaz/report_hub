using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.Import;

public class ImportClientsRequestValidator : AbstractValidator<ImportClientRequest>
{
    private readonly IValidator<ImportDTO> _importDtoValidator;

    public ImportClientsRequestValidator(IValidator<ImportDTO> importDtoValidator)
    {
        _importDtoValidator = importDtoValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.ImportDTO)
            .SetValidator(_importDtoValidator);
        RuleFor(x => x.ImportDTO.File.FileName)
            .Must(fileName => string.Equals(Path.GetExtension(fileName), Constants.File.Extension.Excel, StringComparison.OrdinalIgnoreCase))
            .WithMessage(Constants.Validation.Import.InvalidFileExtension);
    }
}
