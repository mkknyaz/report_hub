const scriptName = "07_create_Item";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Item", {
    collation: {
        locale: "en"
    }
});

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

const names = [
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

const clientIds = [
    UUID("ea94747b-3d45-46d6-8775-bf27eb5da02b"),
    UUID("866eb606-d074-4237-bcf2-aa7798002f7f"),
    UUID("5cb0b8ed-45f4-4432-9ff7-3a9f896362f9"),
    UUID("15de1dcc-98c2-4463-85ed-b36a6a31445a"),
    UUID("e1e39dd5-1ec0-4f9a-b765-d6dc25f0d9a7")
]

const currencyIds = [
    UUID("c1ce0c2a-6701-4d66-95d3-812fa9b2ca08"),
    UUID("04d123f0-dc7e-4b92-829c-dffd1ef0b89a"),
    UUID("45d6d081-e362-4a9d-996f-c144d944635d"),
    UUID("fd76eaab-194a-4e44-a4f8-3eed74c729c8"),
    UUID("f3cc7604-0d40-446e-86fe-e55b103d35b5")
];


const currencyCodes = ["USD", "EUR", "JPY", "BYN", "PLN"]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

const items = [];
const itemCount = 10;

for (let i = 0; i < itemCount; i++) {
    var index = NumberInt(i / 2);
    var nameIndex = getRandomInt(names.length);

    items.push({
        _id: itemIds[i],
        ClientId: clientIds[index],
        Name: names[nameIndex],
        Description: descriptions[nameIndex],
        Price: NumberDecimal((Math.random() * 2000 + 100).toFixed(2)),
        CurrencyId: currencyIds[index],
        CurrencyCode: currencyCodes[index],
        IsDeleted: false
    });
}

const opt = items.map(item => ({
    replaceOne: {
        filter: { _id: item._id },
        replacement: item,
        upsert: true
    }
}));
db.Item.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All items are inserted successfully!");