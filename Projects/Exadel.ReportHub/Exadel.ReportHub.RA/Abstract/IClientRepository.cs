﻿using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IClientRepository
{
    Task AddManyAsync(IEnumerable<Client> clients, CancellationToken cancellationToken);

    Task AddAsync(Client client, CancellationToken cancellationToken);

    Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Client>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<IList<Client>> GetAsync(CancellationToken cancellationToken);

    Task<string> GetCurrencyAsync(Guid id, CancellationToken cancellationToken);

    Task<string> GetNameAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateNameAsync(Guid id, string name, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
}
