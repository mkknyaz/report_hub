using Exadel.ReportHub.Handlers.Validators;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Update;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.UpdateCustomerDto)
           .ChildRules(child =>
           {
               child.RuleLevelCascadeMode = CascadeMode.Stop;

               child.RuleFor(x => x.Name)
                   .SetValidator(new NameValidator());

               child.RuleFor(x => x.Country)
                   .SetValidator(new CountryValidator());
           });
    }
}
