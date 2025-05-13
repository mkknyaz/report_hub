const scriptName = "26_update_Plan_CreatePlanIndexes";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Plan.createIndex(
    { ClientId: 1, StartDate: 1, EndDate: 1, ItemId: 1 },
    {
        partialFilterExpression: { IsDeleted: false },
        background: true
    }
);

db.Plan.createIndex(
    { ClientId: 1, EndDate: 1},
    {
        partialFilterExpression: { IsDeleted: false },
        background: true
    }
);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});
print("Plan indexes created successfully!");