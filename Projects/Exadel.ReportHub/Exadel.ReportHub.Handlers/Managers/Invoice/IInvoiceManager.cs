using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Handlers.Managers.Invoice;

public interface IInvoiceManager
{
    Task<InvoiceDTO> CreateInvoiceAsync(CreateInvoiceDTO createInvoiceDto, CancellationToken cancellationToken);

    Task<IList<InvoiceDTO>> CreateInvoicesAsync(IEnumerable<CreateInvoiceDTO> createInvoiceDtos, CancellationToken cancellationToken);
}
