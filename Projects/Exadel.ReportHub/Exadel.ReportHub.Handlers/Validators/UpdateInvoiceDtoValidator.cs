using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class UpdateInvoiceDtoValidator : AbstractValidator<UpdateInvoiceDTO>
{
    public UpdateInvoiceDtoValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x)
          .ChildRules(child =>
          {
              child.RuleLevelCascadeMode = CascadeMode.Stop;

              child.RuleFor(x => x.IssueDate)
                  .NotEmpty()
                  .LessThan(DateTime.UtcNow)
                  .WithMessage(Constants.Validation.Invoice.IssueDateErrorMessage);
              child.RuleFor(x => x.IssueDate.TimeOfDay)
                  .Equal(TimeSpan.Zero)
                  .WithMessage(Constants.Validation.Invoice.TimeComponentErrorMassage);

              child.RuleFor(x => x.DueDate)
                  .NotEmpty()
                  .GreaterThan(x => x.IssueDate)
                  .WithMessage(Constants.Validation.Invoice.DueDateErrorMessage);
              child.RuleFor(x => x.DueDate.TimeOfDay)
                  .Equal(TimeSpan.Zero)
                  .WithMessage(Constants.Validation.Invoice.TimeComponentErrorMassage);

              child.RuleFor(x => x.PaymentStatus)
                  .IsInEnum();

              child.RuleFor(x => x.BankAccountNumber)
                  .NotEmpty()
                  .Length(Constants.Validation.Invoice.BankAccountNumberMinLength, Constants.Validation.Invoice.BankAccountNumberMaxLength)
                  .Matches(@"^[A-Z]{2}\d+$")
                  .WithMessage(Constants.Validation.Invoice.BankAccountNumberErrorMessage);
          });
    }
}
