using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ClientRepository : BaseRepository, IClientRepository
{
    private static readonly FilterDefinitionBuilder<Client> _filterBuilder = Builders<Client>.Filter;

    public ClientRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync<Client>(id, cancellationToken);
    }
}
