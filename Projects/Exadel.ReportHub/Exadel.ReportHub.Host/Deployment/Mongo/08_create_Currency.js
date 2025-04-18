const scriptName = "08_create_Currency";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Currency", {
    collation: {
        locale: "en"
    }
});

const currencies = [
    {
        _id: UUID("c1ce0c2a-6701-4d66-95d3-812fa9b2ca08"),
        CurrencyCode: "USD"
    },
    {
        _id: UUID("04d123f0-dc7e-4b92-829c-dffd1ef0b89a"),
        CurrencyCode: "EUR"
    },
    {
        _id: UUID("45d6d081-e362-4a9d-996f-c144d944635d"),
        CurrencyCode: "JPY"
    },
    {
        _id: UUID("fd76eaab-194a-4e44-a4f8-3eed74c729c8"),
        CurrencyCode: "BYN"
    },
    {
        _id: UUID("f3cc7604-0d40-446e-86fe-e55b103d35b5"),
        CurrencyCode: "PLN"
    }
];

const opt = currencies.map(currency => ({
    replaceOne: {
        filter: { _id: currency._id },
        replacement: currency,
        upsert: true
    }
}));
db.Currency.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All items are inserted successfully!");