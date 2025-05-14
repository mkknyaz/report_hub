using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Import;

public class ImportCustomersRequestValidator : AbstractValidator<ImportCustomersRequest>
{
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<ImportDTO> _importDtoValidator;

    public ImportCustomersRequestValidator(IClientRepository clientRepository, IValidator<ImportDTO> importDtoValidator)
    {
        _clientRepository = clientRepository;
        _importDtoValidator = importDtoValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);

        RuleFor(x => x.ImportDto)
            .SetValidator(_importDtoValidator);

        RuleFor(x => x.ImportDto.File.FileName)
            .Must(fileName => string.Equals(Path.GetExtension(fileName), Constants.File.Extension.Excel, StringComparison.OrdinalIgnoreCase))
            .WithMessage(Constants.Validation.Import.InvalidFileExtension);
    }
}
