const scriptName = "06_create_UserAssignment";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("UserAssignment", {
    collation: {
        locale: "en"
    }
});
db.UserAssignment.createIndex(
    { UserId: 1, ClientId: 1, Role: 1 },
    {
        unique: true,
        background: true
    });
db.UserAssignment.createIndex(
    { UserId: 1, Role: 1 },
    {
        background: true
    });

const userAssignmentIds = [
    UUID("72f70b7b-5bc8-4b11-85a9-2c9002d3e6fa"),
    UUID("9de19d92-e5c6-410a-8927-38da79e670db"),
    UUID("bec2c596-94a2-4dc3-a404-46786d9da84b"),
    UUID("e57c4af3-5029-40e7-b297-d9e9ceb08fcf"),
    UUID("cbbdadf4-1355-49e3-8d7c-8cfe82d005c8"),
    UUID("fbaafb2d-6a2c-4c71-a0b8-bd9d2f19c7f0"),
    UUID("3040f85c-34a3-4719-aa2a-94be8c087879"),
    UUID("d6955423-fb33-4305-8ef0-b87811cc8ce5"),
    UUID("a7eb02f4-836f-4ca1-b7c3-8abfb596a850"),
    UUID("e6c28f30-2225-4da8-9829-fc5bb8a65f9e"),
    UUID("861ac7bf-244e-450a-baaa-3d287aa8ab77"),
    UUID("b4609b05-2487-4beb-a1a2-8c20343d8c66"),
    UUID("dd7444de-0861-4ef9-9bbc-31be877676ee"),
    UUID("ae696067-e94c-46dc-a3ca-7db9a2ba03fb"),
    UUID("132e2e02-2fac-42df-9c7e-0b7c5cb1b2bc"),
    UUID("e4028079-f38e-40ab-9edf-5d35f884e76c"),
    UUID("d63575d6-21cd-4e54-a47a-8310b1797b61"),
    UUID("59718017-cad4-4ae5-b827-1ee278b0f99a"),
    UUID("222a644b-6446-44bc-9b8e-8010e498f0b3"),
    UUID("d5f65b79-7410-420e-b113-7633338f5f82")
]

const userIds = [
    UUID("ae04b52b-0daa-4e79-b332-616c47ed3cd7"),
    UUID("c5b92a8e-6528-4f13-bff2-7b84cdc4d721"),
    UUID("991dd59d-e249-491d-846c-77c2622cb3de"),
    UUID("2d66669f-8254-4300-be5c-fd5f15da1fe2"),
    UUID("1aca144a-5536-482b-8850-bc2ecac93582"),
    UUID("11b69eb1-a4fd-4d06-a42e-dd654d538e02"),
    UUID("ef107186-19d9-4124-a60e-9f95db89dc89"),
    UUID("8d31447f-bc88-4588-85c3-274c65ba0975"),
    UUID("1343eb98-18e1-4f0b-8ee3-a8e0455e59e7"),
    UUID("fabefb5b-fe77-4513-800c-7ee647b4ac09")
]

const globalClientId = UUID("e47501a8-547b-4dc4-ba97-e65ccfc39477");

const clientIds = [
    UUID("ea94747b-3d45-46d6-8775-bf27eb5da02b"),
    UUID("866eb606-d074-4237-bcf2-aa7798002f7f"),
    UUID("5cb0b8ed-45f4-4432-9ff7-3a9f896362f9"),
    UUID("15de1dcc-98c2-4463-85ed-b36a6a31445a"),
    UUID("e1e39dd5-1ec0-4f9a-b765-d6dc25f0d9a7")
]

const globalRoles = [
    "Regular",
    "SuperAdmin"
]

const clientRoles = [
    "ClientAdmin"
]

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

const userAssignments = [];
const userAssignmentCount = 20;

for (let i = 0; i < userAssignmentCount / 2; i++) {
    userAssignments.push({
        _id: userAssignmentIds[i],
        UserId: userIds[i],
        ClientId: globalClientId,
        Role: globalRoles[getRandomInt(globalRoles.length)]
    });
}

for (let i = userAssignmentCount / 2; i < userAssignmentCount; i++) {
    userAssignments.push({
        _id: userAssignmentIds[i],
        UserId: userIds[i - userAssignmentCount / 2],
        ClientId: clientIds[NumberInt((i - userAssignmentCount / 2) / 2)],
        Role: clientRoles[getRandomInt(clientRoles.length)]
    });
}

const opt = userAssignments.map(userAssignment => ({
    replaceOne: {
        filter: { _id: userAssignment._id },
        replacement: userAssignment,
        upsert: true
    }
}));

db.UserAssignment.bulkWrite(opt);


db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All user assignments are inserted successfully!");