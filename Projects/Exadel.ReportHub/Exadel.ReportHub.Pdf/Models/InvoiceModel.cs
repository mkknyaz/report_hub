using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Pdf.Models;

public class InvoiceModel
{
    public string ClientName { get; set; }

    public string CustomerName { get; set; }

    public string InvoiceNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string BankAccountNumber { get; set; }

    public IList<ItemDTO> Items { get; set; }
}
