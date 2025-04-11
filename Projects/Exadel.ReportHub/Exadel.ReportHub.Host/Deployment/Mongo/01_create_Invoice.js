const scriptName = "01_create_Invoice";
const version = NumberInt(2);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Invoice", {
    collation: {
        locale: "en"
    }
});

const clientIds = [
    UUID("ea94747b-3d45-46d6-8775-bf27eb5da02b"),
    UUID("866eb606-d074-4237-bcf2-aa7798002f7f"),
    UUID("5cb0b8ed-45f4-4432-9ff7-3a9f896362f9"),
    UUID("15de1dcc-98c2-4463-85ed-b36a6a31445a"),
    UUID("e1e39dd5-1ec0-4f9a-b765-d6dc25f0d9a7")
]

const customerIds = [
    UUID("f89e1e75-d61c-4c51-b0be-c285500988cf"),
    UUID("e1509ec2-2b05-406f-befa-149f051586a9"),
    UUID("6d024627-568b-4d57-b477-2274c9d807b9"),
    UUID("ba045076-4837-47ab-80d5-546192851bab"),
    UUID("ba18cc29-c7ff-48c4-9b7b-456bcef231d0")
]

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
    UUID("971b2e38-cd50-412b-a570-bbdb94bb6736"),
]

const itemIds = [
    UUID("895ddfbd-4c6d-4c32-bd5c-02e516771fe5"),
    UUID("d28806ac-e997-49cf-a9f6-428b961ed98a"),
    UUID("fe9035a0-f607-436d-96ca-4f0b7bc6f65a"),
    UUID("f0a0ac3d-f9e5-4a1d-8938-e47ff2edd4f0"),
    UUID("43eb3991-f067-4db2-9a1a-0a7c7b9d783e"),
    UUID("b65afbd7-9986-49ac-81a8-8320dd6444d6"),
    UUID("56c2eb9e-67b5-45e0-b04c-901365a65194"),
    UUID("c634052e-f666-4a55-bcc2-840b02d7a6c1"),
    UUID("b205424a-59b4-4d5f-807f-1e14ff6bf5a7"),
    UUID("e8d6bbf9-a561-4a07-bd87-1431a91f9451"),
    UUID("cd583e6b-17aa-4e77-b23a-11184c44d839"),
    UUID("fe4c5d5f-0114-4e10-bec7-36759a6bd75d"),
    UUID("63c50adb-d621-4fbd-9014-e39a6b69406f"),
    UUID("2d4021ca-6b09-4a51-8e22-966c6ac0a834"),
    UUID("97e3e66d-b28a-45b4-9015-79af9ef51816"),
    UUID("b886d60b-549a-4bea-9e78-e450e9ded1d5"),
    UUID("25a5b444-a09c-49c6-8268-8fd3285b2e56"),
    UUID("b395a542-3b76-4af1-8536-d2982758b984"),
    UUID("fd58070d-fd4d-4456-ae84-8805920989ff"),
    UUID("071295d6-47f8-4ea2-8c72-6a2c910acd35")
]

const itemNames = [
    "Car",
    "Development",
    "Consulting Service",
    "Wholesale purchase",
    "Financial service"
];

const descriptions = [
    "A high-quality vehicle equipped with modern technology for comfort and safety.",
    "Custom software development services tailored to meet specific business needs and foster innovation.",
    "Professional consulting services aimed at strategic growth and effective process optimization.",
    "Bulk purchasing options offering competitive pricing and reliable supply chain management.",
    "Comprehensive financial services including investment advisory, capital management, and risk assessment."
];

const bankAccountNumbers = [
    "PL359459402653871205990733",
    "DE197389122734561028993857",
    "BY849012345678901234567890",
    "GE021987654321098765432109",
    "PL546781234098765432107654"
]

const paymentStatuses = [
    "Unpaid",
    "Pending",
    "Overdue",
    "PartiallyPaid",
    "Paid"
]

const currencies = ["USD", "EUR", "JPY", "INR", "GBP", "BYN", "PLN"]

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

function generateIssueDate() {
    return ISODate(randomDate(new Date("2010-01-01T00:00:00Z"), new Date()).toISOString());
}

function generateDueDate(issueDate) {
    return ISODate(new Date(issueDate.getTime() + (getRandomInt(80) + 10) * 86400000).toISOString());
}

function generateRandomItem(clientId, currency, id) {
    const index = getRandomInt(itemNames.length);
    return {
        _id: id,
        ClientId: clientId,
        Name: itemNames[index],
        Description: descriptions[index],
        Price: NumberDecimal((Math.random() * 2000 + 100).toFixed(2)),
        Currency: currency
    };
}

function generateItems(clientId, currency, index) {
    const items = [];
    var count = 2;

    for (let i = 0; i < count; i++) {
        let id = itemIds[index * 2 + i]
        items.push(generateRandomItem(clientId, currency, id));
    }
    return items;
}

const invoices = [];
const invoiceCount = 10;

for (let i = 0; i < invoiceCount; i++) {
    const index = getRandomInt(clientIds.length);
    const newClientId = clientIds[index];
    const newCustomerId = customerIds[getRandomInt(customerIds.length)];
    const issueDate = generateIssueDate();
    const dueDate = generateDueDate(issueDate);
    const currency = currencies[getRandomInt(currencies.length)];
    const bankAccountNumber = bankAccountNumbers[index]

    const items = generateItems(newClientId, currency, i);

    let totalAmount = 0;
    items.forEach(function (item) {
        totalAmount += parseFloat(item.Price.toString());
    });

    invoices.push({
        _id: invoiceIds[i],
        ClientId: newClientId,
        CustomerId: newCustomerId,
        InvoiceNumber: generateInvoiceNumber(i),
        IssueDate: issueDate,
        DueDate: dueDate,
        Amount: totalAmount.toFixed(2),
        Currency: currency,
        PaymentStatus: paymentStatuses[getRandomInt(paymentStatuses.length)],
        BankAccountNumber: bankAccountNumber,
        Items: items
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