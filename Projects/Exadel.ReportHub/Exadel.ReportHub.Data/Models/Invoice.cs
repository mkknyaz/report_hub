using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Data.Enums;

namespace Exadel.ReportHub.Data.Models;

public class Invoice : IDocument, ISoftDeletable
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid CustomerId { get; set; }

    public string InvoiceNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string BankAccountNumber { get; set; }

    public IList<Guid> ItemIds { get; set; }

    public bool IsDeleted { get; set; }
}
