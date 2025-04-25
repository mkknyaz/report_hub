const scriptName = "12_create_AuditReport";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("AuditReport", {
    collation: { locale: "en" }
});

auditReports = [
    {
        _id: UUID("5a2c0708-1c0d-429b-9a35-98a3fa3a991d"),
        UserId: UUID("c5b92a8e-6528-4f13-bff2-7b84cdc4d721"),
        Properties: {
            InvoiceId: UUID("d312a57c-5ada-408b-918a-8a39bd90213f")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: true
    },
    {
        _id: UUID("45f3d5f8-8283-42d5-8d44-4b79a7d2cf3b"),
        UserId: UUID("8d31447f-bc88-4588-85c3-274c65ba0975"),
        Properties: {
            InvoiceId: UUID("4474d021-cbf8-4942-8411-6eee1a1a82e9")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: false
    },
    {
        _id: UUID("1b90ffec-bebf-4f5b-8a75-6d16a5ab16ad"),
        UserId: UUID("2d66669f-8254-4300-be5c-fd5f15da1fe2"),
        Properties: {
            InvoiceId: UUID("6ccdf0be-e9b7-44d6-820c-b2d0773aafc7")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: true
    },
    {
        _id: UUID("7d927073-41f0-42a4-b5f9-6a503d9ca8b3"),
        UserId: UUID("c5b92a8e-6528-4f13-bff2-7b84cdc4d721"),
        Properties: {
            InvoiceId: UUID("6ccdf0be-e9b7-44d6-820c-b2d0773aafc7")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: false
    },
    {
        _id: UUID("2340ffac-3e12-4a51-b2ab-015b7de7e5f7"),
        UserId: UUID("2d66669f-8254-4300-be5c-fd5f15da1fe2"),
        Properties: {
            InvoiceId: UUID("7d580631-c4fe-4c0f-9dc0-d57909fbf58d")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: true
    },
    {
        _id: UUID("45f5f1da-f433-4c9e-8137-96f960fc75bb"),
        UserId: UUID("2d66669f-8254-4300-be5c-fd5f15da1fe2"),
        Properties: {
            InvoiceId: UUID("7d580631-c4fe-4c0f-9dc0-d57909fbf58d")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: false
    },
    {
        _id: UUID("3a2f2e6c-d37a-437b-9c80-dbc267b9b95e"),
        UserId: UUID("991dd59d-e249-491d-846c-77c2622cb3de"),
        Properties: {
            InvoiceId: UUID("5827a66e-b7f5-4490-b2eb-cc4719bb462c")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: true
    },
    {
        _id: UUID("a28f24f6-d6b9-4d5c-91a5-d39bb1f84eaf"),
        UserId: UUID("ef107186-19d9-4124-a60e-9f95db89dc89"),
        Properties: {
            InvoiceId: UUID("6d0658bd-f2b6-44cd-ad95-49fe8d5d811f")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: false
    },
    {
        _id: UUID("ea33d7f5-35b8-4f5d-8d4e-e3f746dadaf0"),
        UserId: UUID("1aca144a-5536-482b-8850-bc2ecac93582"),
        Properties: {
            InvoiceId: UUID("6d0658bd-f2b6-44cd-ad95-49fe8d5d811f")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: true
    },
    {
        _id: UUID("6f51e1d2-dc9f-490a-bf2b-59f505bf8426"),
        UserId: UUID("fabefb5b-fe77-4513-800c-7ee647b4ac09"),
        Properties: {
            InvoiceId: UUID("971b2e38-cd50-412b-a570-bbdb94bb6736")
        },
        TimeStamp: new Date(),
        Action: "ExportInvoice",
        IsSuccess: false
    }
];

const opt = auditReports.map(report => ({
    replaceOne: {
        filter: { _id: report._id },
        replacement: report,
        upsert: true
    }
}));

db.AuditReport.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("AuditReport collection created.");