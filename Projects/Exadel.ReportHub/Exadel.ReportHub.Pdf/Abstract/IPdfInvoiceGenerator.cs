using Exadel.ReportHub.Pdf.Models;

namespace Exadel.ReportHub.Pdf.Abstract;

public interface IPdfInvoiceGenerator
{
    Task<Stream> GenerateAsync(InvoiceModel invoice, CancellationToken cancellationToken);
}
