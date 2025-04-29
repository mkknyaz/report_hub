const scriptName = "14_update_Client_CreateClientIdIndex";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.customers.createIndex(
    { ClientId: 1 },
    { partialFilterExpression: { IsDeleted: false } }
);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Index created successfully!");