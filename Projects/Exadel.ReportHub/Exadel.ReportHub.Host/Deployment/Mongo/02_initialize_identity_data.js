const scriptName = "02_initialize_identity_data";
const version = NumberInt(1);
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

db.IdentityResource.insertMany([
    {
        _id: UUID(),
        Name: "openid",
        DisplayName: "User identifier",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID(),
        Name: "profile",
        DisplayName: "User profile",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID(),
        Name: "email",
        DisplayName: "Email address",
        Enabled: true,
        Required: true
    },
    {
        _id: UUID(),
        Name: "role",
        DisplayName: "User role",
        Enabled: true,
        Required: true
    }
]);

db.createCollection("ApiScope", {
    collation: {
        locale: "en"
    }
});

db.ApiScope.insertOne(
    {
        _id: UUID(),
        Name: "report_hub_api",
        DisplayName: "Full access to Report Hub API",
        Enabled: true
    });

db.createCollection("ApiResource", {
    collation: {
        locale: "en"
    }
});

db.ApiResource.insertOne(
    {
        _id: UUID(),
        Name: "report_hub_api",
        DisplayName: "Report Hub API",
        Scopes: ["report_hub_api"],
        Enabled: true
    });

db.createCollection("Client", {
    collation: {
        locale: "en"
    }
});

db.Client.insertMany([
    {
        _id: UUID(),
        ClientId: "report_hub_service",
        ClientName: "Report Hub Service",
        AllowedGrantTypes: ["client_credentials"],
        ClientSecrets: [{ Value: reportHubServiceClientSecret }],
        AllowedScopes: ["report_hub_api"]
    },
    {
        _id: UUID(),
        ClientId: "report_hub_resource_owner",
        ClientName: "Report Hub Resource Owner",
        AllowedGrantTypes: ["resource_owner_password"],
        AllowedScopes: ["report_hub_api"]
    }
]);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All identity data is initialized successfully!");