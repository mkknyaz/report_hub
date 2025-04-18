const scriptName = "09_create_Plan";
const version = NumberInt(2);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Plan", {
    collation: {
        locale: "en"
    }
});

const planIds = [
    UUID("32d981d3-13b5-4934-ad59-2152860096eb"),
    UUID("7e9cfe63-9210-4f7a-9c19-e48644cd0693"),
    UUID("5eaf7df1-7ad0-4fcd-8955-e28dca87bee9"),
    UUID("6239e3b1-6abc-44ff-8eb5-2f596c0fffeb"),
    UUID("da3948be-c784-4b10-8b81-1f767fa0792f"),
    UUID("74bf7e1e-9642-49ed-8b3e-612bc06437dc"),
    UUID("ec3fdaac-6034-433c-99a0-811e78b01b7d"),
    UUID("25573254-cb4d-4da9-97b3-4b6611d498f2"),
    UUID("8aaeaa7e-bfed-4767-8d8a-ed1d02e45700"),
    UUID("7e555a29-2c7a-4400-a428-ec46a1b970f9")
]

const clientIds = [
    UUID("ea94747b-3d45-46d6-8775-bf27eb5da02b"),
    UUID("866eb606-d074-4237-bcf2-aa7798002f7f"),
    UUID("5cb0b8ed-45f4-4432-9ff7-3a9f896362f9"),
    UUID("15de1dcc-98c2-4463-85ed-b36a6a31445a"),
    UUID("e1e39dd5-1ec0-4f9a-b765-d6dc25f0d9a7"),
    UUID("d728b231-0b5d-4c90-a2d4-675cbcb64ff2"),
    UUID("4e1f0ed6-0915-48cd-9bf0-eb804e7a919e"),
    UUID("b40ef306-6ac2-4fa8-b703-df291799feef"),
    UUID("00c1df50-320e-447b-8b94-7b2fab0fcf58"),
    UUID("31e52122-ea93-448a-8827-fb5f079cbd1a")
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
]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

function getRandomAmount() {
    return getRandomInt(100) + 1;
}

function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}

function generateStartDate() {
    return ISODate(randomDate(new Date("2025-04-15T00:00:00Z"), new Date()).toISOString());
}

function generateEndDate(StartDate) {
    return ISODate(new Date(StartDate.getTime() + (getRandomInt(80) + 10) * 86400000).toISOString());
}

const plans = [];
const planCount = 10;

for (let i = 0; i < 10; i++) {
    const start = generateStartDate();
    const end = generateEndDate(start);
    plans.push({
        _id: planIds[i],
        ClientId: clientIds[i],
        ItemId: itemIds[i],
        Amount: getRandomAmount(),
        StartDate: start,
        EndDate: end,
        IsDeleted: false
    });
}

const opt = plans.map(plan => ({
    replaceOne: {
        filter: { _id: plan._id },
        replacement: plan,
        upsert: true
    }
}));

db.Plan.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All plans are inserted successfully!");