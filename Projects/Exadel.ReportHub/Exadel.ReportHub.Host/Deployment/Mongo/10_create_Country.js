const scriptName = "10_create_Country";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Country", {
    collation: {
        locale: "en"
    }
});

countries = [
    {
        _id: UUID("5b182ba0-39bf-49c3-812d-cd3ce44bde10"),
        Name: "Poland",
        CurrencyId: UUID("f3cc7604-0d40-446e-86fe-e55b103d35b5"),
        CurrencyCode: "PLN"
    },
    {
        _id: UUID("5e8b6266-c097-468d-97c1-026457ab22a0"),
        Name: "USA",
        CurrencyId: UUID("c1ce0c2a-6701-4d66-95d3-812fa9b2ca08"),
        CurrencyCode: "USD"
    },
    {
        _id: UUID("a4cb75df-6cd1-4a77-a6ed-4e5aa7da85fd"),
        Name: "Italy",
        CurrencyId: UUID("04d123f0-dc7e-4b92-829c-dffd1ef0b89a"),
        CurrencyCode: "EUR"
    },
    {
        _id: UUID("cae4bb82-26f5-4820-b5aa-1343879fb43a"),
        Name: "France",
        CurrencyId: UUID("04d123f0-dc7e-4b92-829c-dffd1ef0b89a"),
        CurrencyCode: "EUR"
    },
    {
        _id: UUID("10accbc3-4c41-4840-aa9a-11b23f0e0d99"),
        Name: "Czech",
        CurrencyId: UUID("c3a29e5d-8421-4b78-9b88-abb692709441"),
        CurrencyCode: "CZK"
    },
    {
        _id: UUID("b436342d-4b4a-406a-bdde-40d4fe77a381"),
        Name: "Bulgaria",
        CurrencyId: UUID("fd76eaab-194a-4e44-a4f8-3eed74c729c8"),
        CurrencyCode: "BGN"
    }
];

const opt = countries.map(country => ({
    replaceOne: {
        filter: { _id: country._id },
        replacement: country,
        upsert: true
    }
}));

db.Country.bulkWrite(opt);


db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All countries are inserted successfully!");