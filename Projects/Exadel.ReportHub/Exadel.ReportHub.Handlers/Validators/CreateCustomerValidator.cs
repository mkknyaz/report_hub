using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDTO>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<UpdateCustomerDTO> _updateCustomerValidator;

    public CreateCustomerValidator(ICustomerRepository customerRepository, IClientRepository clientRepository, IValidator<UpdateCustomerDTO> updateCustomerValidator)
    {
        _customerRepository = customerRepository;
        _clientRepository = clientRepository;
        _updateCustomerValidator = updateCustomerValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x)
           .SetValidator(_updateCustomerValidator);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(Constants.Validation.Email.IsInvalid)
            .MustAsync(async (email, cancellationToken) => !await _customerRepository.EmailExistsAsync(email, cancellationToken))
            .WithMessage(Constants.Validation.Email.IsTaken);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);
    }
}
