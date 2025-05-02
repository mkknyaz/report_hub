const scriptName = "10_create_Country";
const version = NumberInt(3);

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
    }, 
    {
        _id: UUID("1d767405-3204-421f-9565-b2ebc7db0785"),
        "Name": "Japan",
        "CurrencyId": UUID("45d6d081-e362-4a9d-996f-c144d944635d"),
        "CurrencyCode": "JPY"
    },
    {
        _id: UUID("d42db77e-0c68-4053-8c67-69c5f1441c3f"),
        Name: "Denmark",
        CurrencyId: UUID("3169e5d6-6346-45ca-91d8-dd74d4b545ca"),
        CurrencyCode: "DKK"
    },
    {
        _id: UUID("1b36d1b0-6e68-48a0-bcfd-0c9d808c9a5d"),
        Name: "United Kingdom",
        CurrencyId: UUID("23cc2b9f-55cc-47f9-b7bd-29e818517ccc"),
        CurrencyCode: "GBP"
    },
    {
        _id: UUID("264aeed7-85e1-48fd-b5b7-e3b35b4bbd16"),
        Name: "Hungary",
        CurrencyId: UUID("e162e4d0-a79a-41fd-a3eb-d96edcc9d2d0"),
        CurrencyCode: "HUF"
    },
    {
        _id: UUID("b46fc0de-346b-42e0-b5aa-f1b35c6be1d9"),
        Name: "Romania",
        CurrencyId: UUID("ba73c11e-9709-4aa7-882c-9cbd423ed145"),
        CurrencyCode: "RON"
    },
    {
        _id: UUID("bd59ce92-5f17-4c0d-bf90-2ab18b9de275"),
        Name: "Sweden",
        CurrencyId: UUID("6249a156-6db8-4dd0-9c30-1a97cc61fa01"),
        CurrencyCode: "SEK"
    },
    {
        _id: UUID("f1cc0b86-59c2-4377-8fc4-bd6d4f7057ee"),
        Name: "Switzerland",
        CurrencyId: UUID("736709dc-460b-46d9-861f-15b7b3e04856"),
        CurrencyCode: "CHF"
    },
    {
        _id: UUID("2b6aa062-fab3-40f9-b4d6-df8313c7346b"),
        Name: "Iceland",
        CurrencyId: UUID("b9d1648a-ca8c-40e1-b5ed-3f605f0adfff"),
        CurrencyCode: "ISK"
    },
    {
        _id: UUID("36470c52-493e-4f1e-8e6d-6e6d6f3e6f6b"),
        Name: "Norway",
        CurrencyId: UUID("637f5dc9-c855-409d-8daf-36087bd3f71b"),
        CurrencyCode: "NOK"
    },
    {
        _id: UUID("fb70e261-d68f-45cb-acc5-8d7b5460f1b0"),
        Name: "Turkey",
        CurrencyId: UUID("15f447c9-3824-4caa-a277-81000deba730"),
        CurrencyCode: "TRY"
    },
    {
        _id: UUID("fdc938a4-7d15-4e3c-81ec-6d537417c1a7"),
        Name: "Australia",
        CurrencyId: UUID("6acc354b-4ad4-4507-8b94-d27c43a06778"),
        CurrencyCode: "AUD"
    },
    {
        _id: UUID("2ec6f372-e7a7-4e52-9446-640fbde16142"),
        Name: "Brazil",
        CurrencyId: UUID("85824bfb-9847-48e4-b1e2-c9b0d97c63a9"),
        CurrencyCode: "BRL"
    },
    {
        _id: UUID("d0ce4a29-5c3a-4218-85be-0a1591f4f7d7"),
        Name: "Canada",
        CurrencyId: UUID("1bb21106-7b4f-4fd5-9651-adce4c249306"),
        CurrencyCode: "CAD"
    },
    {
        _id: UUID("eb0b1de4-c4bc-4b9b-bf29-0e84f7d4d425"),
        Name: "China",
        CurrencyId: UUID("2ff40074-260f-4eb4-a074-f75e0e385978"),
        CurrencyCode: "CNY"
    },
    {
        _id: UUID("6e6ec80c-e59c-46e0-82a3-d52f1a469cbd"),
        Name: "Hong Kong",
        CurrencyId: UUID("a025f810-3a70-4954-b62d-870c004ecc91"),
        CurrencyCode: "HKD"
    },
    {
        _id: UUID("16871b55-dcdc-4bde-924f-e5e0b7c9a3f2"),
        Name: "Indonesia",
        CurrencyId: UUID("59d6a6ff-3f23-4daa-a5bd-04060d1767e0"),
        CurrencyCode: "IDR"
    },
    {
        _id: UUID("f8372964-1dd4-43dc-8703-bc44e4c73c92"),
        Name: "Israel",
        CurrencyId: UUID("11be352d-768b-414f-b0b6-974538d6536f"),
        CurrencyCode: "ILS"
    },
    {
        _id: UUID("20e3c6e2-55ee-47f2-a626-f72cc0c112b2"),
        Name: "India",
        CurrencyId: UUID("46fe9106-9a6d-4566-8350-941bc3810969"),
        CurrencyCode: "INR"
    },
    {
        _id: UUID("49fa50db-b2e5-4453-a548-19c60919cbf3"),
        Name: "South Korea",
        CurrencyId: UUID("83ac410c-a8a5-477f-bd16-b03d4de7093e"),
        CurrencyCode: "KRW"
    },
    {
        _id: UUID("6e25708a-39db-4903-9f7f-7a4de63a74a8"),
        Name: "Mexico",
        CurrencyId: UUID("49ee26bb-ec43-4aba-929a-782b925af893"),
        CurrencyCode: "MXN"
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