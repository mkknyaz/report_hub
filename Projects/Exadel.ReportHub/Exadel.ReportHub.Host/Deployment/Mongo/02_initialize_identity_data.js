const scriptName = "02_initialize_identity_data";
const version = NumberInt(4);
const reportHubServiceClientSecret = process.env.ReportHubService_ClientSecret

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("IdentityResource", {
    collation: {
        locale: "en"
    }
});
db.IdentityResource.createIndex(
    { Name: 1 },
    {
        unique: true,
        background: true
    });

db.createCollection("ApiScope", {
    collation: {
        locale: "en"
    }
});
db.ApiScope.createIndex(
    { Name: 1 },
    {
        unique: true,
        background: true
    });

db.createCollection("ApiResource", {
    collation: {
        locale: "en"
    }
});
db.ApiResource.createIndex(
    { Name: 1 },
    {
        unique: true,
        background: true
    });
db.ApiResource.createIndex(
    { Scopes: 1 },
    {
        background: true
    });

db.createCollection("IdentityClient", {
    collation: {
        locale: "en"
    }
});
db.IdentityClient.createIndex(
    { ClientId: 1 },
    {
        unique: true,
        background: true
    });

const identityResources = [
    {
        _id: UUID("4b2af67b-cdff-41c6-a17d-019e47d96718"),
        Name: "openid",
        DisplayName: "User identifier",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID("fa240ee8-6a57-452f-8521-0cb570a7c990"),
        Name: "profile",
        DisplayName: "User profile",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID("8bc101d4-5b98-4c3c-ac9b-ae6fb7505ef4"),
        Name: "email",
        DisplayName: "Email address",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID("6a642d1d-2c85-4a40-af50-3749051b433b"),
        Name: "role",
        DisplayName: "User role",
        Enabled: true,
        Required: true
    }
]

const apiScopes = [
    {
        _id: UUID("eb67cadc-1d9e-4652-9c66-7522f8664352"),
        Name: "report_hub_api",
        DisplayName: "Full access to Report Hub API",
        Enabled: true
    }
]

const apiResources = [
    {
        _id: UUID("b090422b-f1ef-474d-98dc-3c0ee5977cc2"),
        Name: "report_hub_api",
        DisplayName: "Report Hub API",
        Scopes: ["report_hub_api"],
        Enabled: true
    }
]

const identityClients = [
    {
        _id: UUID("51b5f945-f1c1-4fa5-a717-4a6ad54f26e6"),
        ClientId: "report_hub_service",
        ClientName: "Report Hub Service",
        AllowedGrantTypes: ["client_credentials"],
        RequireClientSecret: true,
        ClientSecrets: [{ Value: reportHubServiceClientSecret }],
        AllowedScopes: ["report_hub_api"]
    },
    {
        _id: UUID("08928498-5223-453f-8e32-4d24bd37b990"),
        ClientId: "report_hub_resource_owner",
        ClientName: "Report Hub Resource Owner",
        AllowedGrantTypes: ["password"],
        RequireClientSecret: false,
        AllowedScopes: ["report_hub_api"]
    }
]

const optIR = identityResources.map(identityResource => ({
    replaceOne: {
        filter: { _id: identityResource._id },
        replacement: identityResource,
        upsert: true
    }
}));

const optAS = apiScopes.map(apiScope => ({
    replaceOne: {
        filter: { _id: apiScope._id },
        replacement: apiScope,
        upsert: true
    }
}))

const optAR = apiResources.map(apiResource => ({
    replaceOne: {
        filter: { _id: apiResource._id },
        replacement: apiResource,
        upsert: true
    }
}))

const optIC = identityClients.map(identityClient => ({
    replaceOne: {
        filter: { _id: identityClient._id },
        replacement: identityClient,
        upsert: true
    }
}))

db.IdentityResource.bulkWrite(optIR);

db.ApiScope.bulkWrite(optAS);

db.ApiResource.bulkWrite(optAR);

db.IdentityClient.bulkWrite(optIC);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All identity data is initialized successfully!");