const scriptName = "13_update_Client_CustomerIdsUnset";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Client.updateMany({}, { $unset: { CustomerIds: 1 } });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All CustomerIds are unseted successfully!");