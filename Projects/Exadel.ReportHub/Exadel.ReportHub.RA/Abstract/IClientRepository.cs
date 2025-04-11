namespace Exadel.ReportHub.RA.Abstract;

public interface IClientRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
