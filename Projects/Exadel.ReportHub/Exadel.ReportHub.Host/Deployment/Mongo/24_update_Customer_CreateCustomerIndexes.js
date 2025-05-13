const scriptName = "24_update_Customer_CreateCustomerIndexes";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Customer.createIndex(
    { Email: 1 },
    {
        unique: true,
        background: true
    });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Indexes were created successfully!");