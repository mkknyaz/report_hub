using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Update;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    private readonly IValidator<string> _stringValidator;

    public UpdateCustomerValidator(IValidator<string> stringValidator)
    {
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.UpdateCustomerDto)
           .ChildRules(child =>
           {
               child.RuleLevelCascadeMode = CascadeMode.Stop;

               child.RuleFor(x => x.Name)
                   .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);

               child.RuleFor(x => x.Country)
                   .SetValidator(_stringValidator, Constants.Validation.RuleSet.Countries);
           });
    }
}
