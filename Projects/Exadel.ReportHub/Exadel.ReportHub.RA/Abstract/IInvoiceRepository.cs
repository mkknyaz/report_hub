using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IInvoiceRepository
{
    Task AddManyAsync(IEnumerable<Invoice> invoices, CancellationToken cancellationToken);
}
