﻿using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.Identity.Stores;

public class IdentityClientStore(IIdentityRepository identityRepository) : IClientStore
{
    public async Task<Client> FindClientByIdAsync(string clientId)
    {
        var client = await identityRepository.GetClientByIdAsync(clientId, CancellationToken.None);

        client.AlwaysSendClientClaims = true;
        client.AlwaysIncludeUserClaimsInIdToken = true;

        client.ClientClaimsPrefix = string.Empty;

        return client;
    }
}
