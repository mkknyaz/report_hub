const scriptName = "05_create_Client";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Client", {
    collation: {
        locale: "en"
    }
});

const clientIds = [
    UUID("f89e1e75-d61c-4c51-b0be-c285500988cf"),
    UUID("e1509ec2-2b05-406f-befa-149f051586a9"),
    UUID("6d024627-568b-4d57-b477-2274c9d807b9"),
    UUID("ba045076-4837-47ab-80d5-546192851bab"),
    UUID("ba18cc29-c7ff-48c4-9b7b-456bcef231d0"),
    UUID("8595ec4b-0f5b-4b38-9b1a-388844debba6"),
    UUID("d4baebdc-ae9b-42d9-b7bf-cbdbe6abef9e"),
    UUID("c77838e5-1385-4495-bca3-c90f8d70834d"),
    UUID("60c2ba28-3bc7-443a-9b90-1ea84c33a0c6"),
    UUID("b74d6a47-9f18-4dcd-8344-32f39f219881")
]

const customerIds = [
    UUID("f89e1e75-d61c-4c51-b0be-c285500988cf"),
    UUID("e1509ec2-2b05-406f-befa-149f051586a9"),
    UUID("6d024627-568b-4d57-b477-2274c9d807b9"),
    UUID("ba045076-4837-47ab-80d5-546192851bab"),
    UUID("ba18cc29-c7ff-48c4-9b7b-456bcef231d0"),
    UUID("1fddb42d-0436-4123-9448-2821ed38c158"),
    UUID("30f41dcc-fbde-4321-b929-dd332ec27f8d"),
    UUID("ac0fe3cf-9ba1-4bd7-a22c-554972b44ed0"),
    UUID("5223d592-7e90-46b6-939d-9f715d4b5058"),
    UUID("43867fc9-ab6e-43ae-8c2c-97f2575969df"),
    UUID("2b00cf70-2ce9-4787-b4da-5baf62365c16"),
    UUID("802c97bd-2828-42bd-a640-1114eed702e8"),
    UUID("f3e0620c-5bd5-44ba-9ce3-77a6ef5be83d"),
    UUID("1f59b609-adf5-4fd0-94f7-b9c74e6e9572"),
]

const clientNames = [
    "Acme Corp",
    "Globex Corp",
    "Umbrella Corp",
    "Apple Corp",
    "Pfizer",
    "Novavax Comp",
    "Exadel",
    "Umbrella Corp",
    "Unitex",
    "Maxwell"
]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

function randomCustomerIds(index) {
    const count = getRandomInt(4) + 1;
    const customers = [];

    for (let i = 0; i < count; i++) {
        customers.push(customerIds[index + i]);
    }
    return customers;
}

const clients = [];
const clientCount = 10;

for (let i = 0; i < clientCount; i++) {
    clients.push({
        _id: clientIds[i],
        Name: clientNames[i],
        CustomerIds: randomCustomerIds(i)
    });
}

const opt = clients.map(client => ({
    replaceOne: {
        filter: { _id: client._id },
        replacement: client,
        upsert: true
    }
}));
db.Client.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All clients are inserted successfully!");