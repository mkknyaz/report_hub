using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class InvoiceDTO
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid CustomerId { get; set; }

    public string InvoiceNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string BankAccountNumber { get; set; }

    public IList<Guid> ItemIds { get; set; }
}
