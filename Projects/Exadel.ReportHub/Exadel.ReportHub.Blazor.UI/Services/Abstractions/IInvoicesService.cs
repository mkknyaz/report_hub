using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface IInvoicesService
{
    Task<IList<InvoiceDTO>> GetInvoicesByClientIdAsync(Guid clientId);

    Task<InvoiceDTO> GetInvoiceByIdAsync(Guid invoiceId, Guid clientId);

    Task CreateInvoiceAsync(CreateInvoiceDTO dto);

    Task UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDTO dto, Guid clientId);

    Task DeleteInvoiceAsync(Guid invoiceId, Guid clientId);
}
