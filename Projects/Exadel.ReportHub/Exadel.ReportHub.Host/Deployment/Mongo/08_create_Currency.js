const scriptName = "08_create_Currency";
const version = NumberInt(2);

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
        CurrencyCode: "BGN"
    },
    {
        _id: UUID("f3cc7604-0d40-446e-86fe-e55b103d35b5"),
        CurrencyCode: "PLN"
    },
    {
        _id: UUID("c3a29e5d-8421-4b78-9b88-abb692709441"),
        CurrencyCode: "CZK"
    },
    {
        _id: UUID("3169e5d6-6346-45ca-91d8-dd74d4b545ca"),
        CurrencyCode: "DKK"
    },
    {
        _id: UUID("23cc2b9f-55cc-47f9-b7bd-29e818517ccc"),
        CurrencyCode: "GBP"
    },
    {
        _id: UUID("e162e4d0-a79a-41fd-a3eb-d96edcc9d2d0"),
        CurrencyCode: "HUF"
    },
    {
        _id: UUID("ba73c11e-9709-4aa7-882c-9cbd423ed145"),
        CurrencyCode: "RON"
    },
    {
        _id: UUID("6249a156-6db8-4dd0-9c30-1a97cc61fa01"),
        CurrencyCode: "SEK"
    },
    {
        _id: UUID("736709dc-460b-46d9-861f-15b7b3e04856"),
        CurrencyCode: "CHF"
    },
    {
        _id: UUID("b9d1648a-ca8c-40e1-b5ed-3f605f0adfff"),
        CurrencyCode: "ISK"
    },
    {
        _id: UUID("637f5dc9-c855-409d-8daf-36087bd3f71b"),
        CurrencyCode: "NOK"
    },
    {
        _id: UUID("15f447c9-3824-4caa-a277-81000deba730"),
        CurrencyCode: "TRY"
    },
    {
        _id: UUID("6acc354b-4ad4-4507-8b94-d27c43a06778"),
        CurrencyCode: "AUD"
    },
    {
        _id: UUID("85824bfb-9847-48e4-b1e2-c9b0d97c63a9"),
        CurrencyCode: "BRL"
    },
    {
        _id: UUID("1bb21106-7b4f-4fd5-9651-adce4c249306"),
        CurrencyCode: "CAD"
    },
    {
        _id: UUID("2ff40074-260f-4eb4-a074-f75e0e385978"),
        CurrencyCode: "CNY"
    },
    {
        _id: UUID("a025f810-3a70-4954-b62d-870c004ecc91"),
        CurrencyCode: "HKD"
    },
    {
        _id: UUID("59d6a6ff-3f23-4daa-a5bd-04060d1767e0"),
        CurrencyCode: "IDR"
    },
    {
        _id: UUID("11be352d-768b-414f-b0b6-974538d6536f"),
        CurrencyCode: "ILS"
    },
    {
        _id: UUID("46fe9106-9a6d-4566-8350-941bc3810969"),
        CurrencyCode: "INR"
    },
    {
        _id: UUID("83ac410c-a8a5-477f-bd16-b03d4de7093e"),
        CurrencyCode: "KRW"
    },
    {
        _id: UUID("49ee26bb-ec43-4aba-929a-782b925af893"),
        CurrencyCode: "MXN"
    },
    {
        _id: UUID("5f9f0386-2814-421d-af85-a1a76fe576a9"),
        CurrencyCode: "MYR"
    },
    {
        _id: UUID("91437822-65dd-4c8e-a3f2-519ee57c251c"),
        CurrencyCode: "NZD"
    },
    {
        _id: UUID("312a23fc-fec7-41b2-86fe-5d63f490a782"),
        CurrencyCode: "PHP"
    },
    {
        _id: UUID("c174ea3d-64cc-4df9-abcf-44f95b673c41"),
        CurrencyCode: "SGD"
    },
    {
        _id: UUID("fb48e84f-dd0a-4efd-92ee-bbe46664415a"),
        CurrencyCode: "THB"
    },
    {
        _id: UUID("da681acd-1e83-404d-affd-28ae4a28db24"),
        CurrencyCode: "ZAR"
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