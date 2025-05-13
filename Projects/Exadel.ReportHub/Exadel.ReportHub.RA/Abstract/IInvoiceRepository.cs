using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IInvoiceRepository
{
    Task AddManyAsync(IEnumerable<Invoice> invoices, CancellationToken cancellationToken);

    Task AddAsync(Invoice invoice, CancellationToken cancellationToken);

    Task<IList<Invoice>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Invoice> GetByIdAsync(Guid id, Guid clientId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, Guid clientId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string invoiceNumber, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken);

    Task UpdatePaidStatusAsync(Guid id, Guid clientId, CancellationToken cancellationToken);

    Task<long> UpdateOverdueStatusAsync(DateTime date, CancellationToken cancellationToken);

    Task<TotalRevenue> GetTotalAmountByDateRangeAsync(Guid clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task<Dictionary<Guid, int>> GetCountByDateRangeAsync(DateTime startDate, DateTime endDate, Guid clientId, Guid? customerId, CancellationToken cancellationToken);

    Task<OverdueCount> GetOverdueAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Dictionary<Guid, int>> GetClientItemsCountAsync(Guid clientId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);

    Task<Dictionary<Guid, int>> GetPlansActualCountAsync(IEnumerable<Plan> plans, CancellationToken cancellationToken);

    Task<InvoicesReport> GetReportAsync(Guid clientId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
}
