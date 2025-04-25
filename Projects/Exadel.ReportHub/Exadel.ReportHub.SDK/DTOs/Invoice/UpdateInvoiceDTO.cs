using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class UpdateInvoiceDTO
{
    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }
}
