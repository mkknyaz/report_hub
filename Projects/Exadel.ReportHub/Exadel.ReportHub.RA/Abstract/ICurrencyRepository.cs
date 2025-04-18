namespace Exadel.ReportHub.RA.Abstract;

public interface ICurrencyRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<string> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken);
}
