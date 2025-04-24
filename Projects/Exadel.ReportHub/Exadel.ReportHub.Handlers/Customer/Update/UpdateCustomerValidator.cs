using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Update;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    private readonly IValidator<UpdateCustomerDTO> _updateCustomerValidator;

    public UpdateCustomerValidator(IValidator<UpdateCustomerDTO> updateCustomerValidator)
    {
        _updateCustomerValidator = updateCustomerValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.UpdateCustomerDto)
           .SetValidator(_updateCustomerValidator);
    }
}
