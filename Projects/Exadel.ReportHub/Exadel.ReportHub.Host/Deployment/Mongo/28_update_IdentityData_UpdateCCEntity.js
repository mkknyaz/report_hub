const scriptName = "28_update_IdentityData_UpdateCCEntity";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.IdentityClient.updateOne(
    { ClientId: "report_hub_service" },
    {
        $set: {
            Claims: [
                { Type: "sub", Value: "e6a7748e-66e9-4dd6-a497-f330c1ac400e" },
                { Type: "role", Value: "SuperAdmin" }
            ],
            AlwaysSendClientClaims: true,
            AlwaysIncludeUserClaimsInIdToken: true
        }
    }
);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Super admin claims added successfully!");