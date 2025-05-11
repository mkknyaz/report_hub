using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Handlers.Managers.Invoice;

public interface IInvoiceManager
{
    Task<Data.Models.Invoice> GenerateInvoiceAsync(CreateInvoiceDTO invoiceDto, CancellationToken cancellationToken);

    Task<IList<Data.Models.Invoice>> GenerateInvoicesAsync(IEnumerable<CreateInvoiceDTO> invoiceDtos, CancellationToken cancellationToken);
}
