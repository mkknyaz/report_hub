using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IIdentityRepository<TDocument>
    where TDocument : IDocument
{
    Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken);

    Task<TDocument> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
