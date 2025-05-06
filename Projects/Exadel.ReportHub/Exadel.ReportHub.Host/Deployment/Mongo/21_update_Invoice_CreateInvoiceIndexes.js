const scriptName = "21_update_Invoice_CreateInvoiceIndexes";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.Invoice.createIndex(
    { ClientId: 1, IssueDate: 1 },
    { partialFilterExpression: { IsDeleted: false } },
    {
        background: true
    });

db.Invoice.createIndex(
    { ClientId: 1, PaymentStatus: 1 },
    { partialFilterExpression: { IsDeleted: false } },
    {
        background: true
    });

db.Invoice.createIndex(
    { ClientId: 1, InvoiceNumber: 1 },
    {
        unique: true,
        background: true
    });

db.Invoice.createIndex(
    { PaymentStatus: 1, DueDate: 1 },
    { partialFilterExpression: { IsDeleted: false } },
    {
        background: true
    });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Indexes were created successfully!");