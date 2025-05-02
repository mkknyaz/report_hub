const scriptName = "17_update_ExchangeRate_CurrencyRateDateIndex";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.ExchangeRate.dropIndexes(["Currency_1", "RateDate_1"]);

db.ExchangeRate.createIndex(
    { Currency: 1, RateDate: 1},
    {
        unique: true,
        background: true
    });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Index updated successfully!");