const scriptName = "01_create_Invoice";
const version = NumberInt(7);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Invoice", {
    collation: {
        locale: "en"
    }
});

const customerClientIds = [
    {
        Customer: UUID("f89e1e75-d61c-4c51-b0be-c285500988cf"),
        Client: UUID("ea94747b-3d45-46d6-8775-bf27eb5da02b")
    },
    {
        Customer: UUID("e1509ec2-2b05-406f-befa-149f051586a9"),
        Client: UUID("866eb606-d074-4237-bcf2-aa7798002f7f")
    },
    {
        Customer: UUID("6d024627-568b-4d57-b477-2274c9d807b9"),
        Client: UUID("5cb0b8ed-45f4-4432-9ff7-3a9f896362f9")
    },
    {
        Customer: UUID("ba045076-4837-47ab-80d5-546192851bab"),
        Client: UUID("15de1dcc-98c2-4463-85ed-b36a6a31445a")
    },
    {
        Customer: UUID("ba18cc29-c7ff-48c4-9b7b-456bcef231d0"),
        Client: UUID("e1e39dd5-1ec0-4f9a-b765-d6dc25f0d9a7")
    }
];

const invoiceIds = [
    UUID("d312a57c-5ada-408b-918a-8a39bd90213f"),
    UUID("6ccdf0be-e9b7-44d6-820c-b2d0773aafc7"),
    UUID("8cf9424a-8cb4-4d8c-8bcc-095335a0ced9"),
    UUID("e04f2fba-925d-4dfb-bb68-04022d86c478"),
    UUID("7d580631-c4fe-4c0f-9dc0-d57909fbf58d"),
    UUID("4474d021-cbf8-4942-8411-6eee1a1a82e9"),
    UUID("5827a66e-b7f5-4490-b2eb-cc4719bb462c"),
    UUID("6d0658bd-f2b6-44cd-ad95-49fe8d5d811f"),
    UUID("b639fc00-1337-4159-8c76-5c9494643633"),
    UUID("971b2e38-cd50-412b-a570-bbdb94bb6736")
]

const itemIds = [
    UUID("8892462c-df8f-4a6f-b5ce-95e89db220bf"),
    UUID("f50ce8fc-9b5e-47e9-aeb9-7eab03677b8d"),
    UUID("c96844cf-5c1d-4672-b339-4a3eb6a9db8b"),
    UUID("76fb1a23-2f77-4c26-bf45-fc655f7432e6"),
    UUID("f55ce9a9-db00-4634-9759-2f369c4d0df1"),
    UUID("351e5e82-b200-4e5d-8b1c-9e0d2e52ceaa"),
    UUID("5c98227f-e9b7-45dd-bfdb-22dddf384598"),
    UUID("3b9ff6f2-d612-481c-b2a5-17c7c1c1ffb3"),
    UUID("e2b72b14-f334-4ef9-81b5-a86045e39c12"),
    UUID("aacf3867-90bf-422c-b271-540f2d7a157a")
];

const currencyIds = [
    UUID("c1ce0c2a-6701-4d66-95d3-812fa9b2ca08"),
    UUID("04d123f0-dc7e-4b92-829c-dffd1ef0b89a"),
    UUID("45d6d081-e362-4a9d-996f-c144d944635d"),
    UUID("fd76eaab-194a-4e44-a4f8-3eed74c729c8"),
    UUID("f3cc7604-0d40-446e-86fe-e55b103d35b5"),
    UUID("c3a29e5d-8421-4b78-9b88-abb692709441"),
    UUID("3169e5d6-6346-45ca-91d8-dd74d4b545ca"),
    UUID("23cc2b9f-55cc-47f9-b7bd-29e818517ccc"),
    UUID("e162e4d0-a79a-41fd-a3eb-d96edcc9d2d0"),
    UUID("ba73c11e-9709-4aa7-882c-9cbd423ed145")
];

const currencyCodes = [
    "USD",
    "EUR",
    "JPY",
    "BGN",
    "PLN",
    "CZK",
    "DKK",
    "GBP",
    "HUF",
    "RON"
]

const clientBankAccountNumbers = [
    "PL359459402653871205990733",
    "DE197389122734561028993857",
    "BY849012345678901234567890",
    "GE021987654321098765432109",
    "PL546781234098765432107654"
]

const paymentStatuses = [
    "Unpaid",
    "Paid"
]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}

function generateInvoiceNumber(index) {
    var currentDate = new Date();
    var year = currentDate.getFullYear() - getRandomInt(12);
    return "INV" + year + getRandomInt(30) + ("0000000" + (index + 1)).slice(-7);
}

function truncateToOnlyDate(date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

function generateIssueDate() {
    const rawDate = randomDate(new Date("2010-01-01T00:00:00Z"), new Date());
    return ISODate(truncateToOnlyDate(rawDate).toISOString());
}

function generateDueDate(issueDate) {
    const rawDate = new Date(issueDate.getTime() + (getRandomInt(80) + 10) * 86400000);
    return ISODate(truncateToOnlyDate(rawDate).toISOString());
}

const invoices = [];
const invoiceCount = 10;

for (let i = 0; i < invoiceCount; i++) {
    var index = NumberInt(i / 2);
    var issueDate = generateIssueDate();

    invoices.push({
        _id: invoiceIds[i],
        ClientId: customerClientIds[index].Client,
        CustomerId: customerClientIds[index].Customer,
        InvoiceNumber: generateInvoiceNumber(i),
        IssueDate: issueDate,
        DueDate: generateDueDate(issueDate),
        Amount: NumberDecimal((Math.random() * 4000 + 100).toFixed(2)),
        CurrencyId: currencyIds[index],
        CurrencyCode: currencyCodes[index],
        PaymentStatus: paymentStatuses[getRandomInt(paymentStatuses.length)],
        ClientBankAccountNumber: clientBankAccountNumbers[index],
        ItemIds: [itemIds[index * 2], itemIds[index * 2 + 1]],
        IsDeleted: false,
    });
}

const opt = invoices.map(invoice => ({
    replaceOne: {
        filter: { _id: invoice._id },
        replacement: invoice,
        upsert: true
    }
}));
db.Invoice.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All invoices are inserted successfully!");