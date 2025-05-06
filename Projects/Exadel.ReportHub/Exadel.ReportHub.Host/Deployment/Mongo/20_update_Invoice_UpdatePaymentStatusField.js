const scriptName = "20_update_Invoice_UpdatePaymentStatusField";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

const result = db.Invoice.updateMany(
    { paymentStatus: { $in: ["Paid", null] } },
    { $set: { paymentStatus: "PaidOnTime" } }
);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print(`Updated ${result.modifiedCount} invoices to "PaidOnTime".`);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("PaymentStatus filed updated successfully!");