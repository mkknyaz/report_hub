const scriptName = "25_update_Item_CreateItemIndexes";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Item.createIndex(
    { ClientId: 1 },
    {
        partialFilterExpression: { IsDeleted: false },
        background: true
    });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Indexes were created successfully!");