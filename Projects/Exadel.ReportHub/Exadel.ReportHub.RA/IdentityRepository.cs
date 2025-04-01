using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.RA;

public class IdentityRepository<TDocument> : BaseRepository<TDocument>, IIdentityRepository<TDocument>
    where TDocument : IDocument
{
    public IdentityRepository(MongoDbContext context)
        : base(context)
    {
    }
}
