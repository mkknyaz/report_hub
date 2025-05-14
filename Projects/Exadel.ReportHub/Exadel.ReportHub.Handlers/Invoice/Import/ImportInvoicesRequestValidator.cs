using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.Import;

public class ImportInvoicesRequestValidator : AbstractValidator<ImportInvoicesRequest>
{
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<ImportDTO> _importDtoValidator;

    public ImportInvoicesRequestValidator(IClientRepository clientRepository, IValidator<ImportDTO> importDtoValidator)
    {
        _clientRepository = clientRepository;
        _importDtoValidator = importDtoValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.ImportDto)
            .SetValidator(_importDtoValidator);

        RuleFor(x => x.ImportDto.File.FileName)
            .Must(fileName => string.Equals(Path.GetExtension(fileName), Constants.File.Extension.Csv, StringComparison.OrdinalIgnoreCase))
            .WithMessage(Constants.Validation.Import.InvalidFileExtension);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);
    }
}
