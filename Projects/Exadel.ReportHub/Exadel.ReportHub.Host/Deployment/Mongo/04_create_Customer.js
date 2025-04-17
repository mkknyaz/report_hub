const scriptName = "04_create_Customer";
const version = NumberInt(2);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Customer", {
    collation: {
        locale: "en"
    }
});

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
    UUID("43867fc9-ab6e-43ae-8c2c-97f2575969df")
]

const customerNames = [
    "Hudson Kim",
    "Jonathan Allen",
    "Cooper Roberts",
    "Landon Rivera",
    "Colton Rivera",
    "Lucas Watson",
    "Gabriel Lopez",
    "Theo Price",
    "Aiden Gonzalez",
    "Waylon Howard",
]

const countries = ["USA", "Canada", "UK", "Germany", "France", "Australia", "Poland", "Georgia", "Finland", "Korea"]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

const customers = [];
const customerCount = 10;

for (let i = 0; i < customerCount; i++) {
    const country = countries[getRandomInt(countries.length)];

    customers.push({
        _id: customerIds[i],
        Country: country,
        Email: customerNames[i].replace(/\s/g, '').toLowerCase() + "@test.com",
        Name: customerNames[i],
        IsDeleted: false
    });
}

const opt = customers.map(customer => ({
    replaceOne: {
        filter: { _id: customer._id },
        replacement: customer,
        upsert: true
    }
}));
db.Customer.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All customers are inserted successfully!");