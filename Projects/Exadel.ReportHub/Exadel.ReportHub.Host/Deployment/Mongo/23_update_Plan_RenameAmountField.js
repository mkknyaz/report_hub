const scriptName = "23_update_Plan_RenameAmountField";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Plan.updateMany(
    {},
    { $rename: { "Amount": "Count" } }
)

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Field wes renamed successfully!");