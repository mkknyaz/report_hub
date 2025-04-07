const scriptName = "01_create_Invoice";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("Invoice", {
    collation: {
        locale: "en"
    }
});

db.Invoice.insertMany([
    {
        _id: UUID(),
        InvoiceId: "INV2025002",
        IssueDate: ISODate("2025-01-20T00:00:00Z"),
        DueDate: ISODate("2025-02-20T00:00:00Z"),
        Amount: NumberDecimal("500.00"),
        Currency: "USD",
        PaymentStatus: "Unpaid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024001",
        IssueDate: ISODate("2024-04-28T00:00:00Z"),
        DueDate: ISODate("2024-06-19T00:00:00Z"),
        Amount: NumberDecimal("98917.28"),
        Currency: "JPY",
        PaymentStatus: "Overdue"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025005",
        IssueDate: ISODate("2025-08-09T00:00:00Z"),
        DueDate: ISODate("2025-09-17T00:00:00Z"),
        Amount: NumberDecimal("47418.14"),
        Currency: "USD",
        PaymentStatus: "Unpaid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024002",
        IssueDate: ISODate("2024-08-12T00:00:00Z"),
        DueDate: ISODate("2024-09-05T00:00:00Z"),
        Amount: NumberDecimal("38510.38"),
        Currency: "CAD",
        PaymentStatus: "Cancelled"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024003",
        IssueDate: ISODate("2024-08-20T00:00:00Z"),
        DueDate: ISODate("2024-09-17T00:00:00Z"),
        Amount: NumberDecimal("17775.52"),
        Currency: "INR",
        PaymentStatus: "Partially Paid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024004",
        IssueDate: ISODate("2024-10-24T00:00:00Z"),
        DueDate: ISODate("2024-12-10T00:00:00Z"),
        Amount: NumberDecimal("35625.06"),
        Currency: "GBP",
        PaymentStatus: "Paid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024005",
        IssueDate: ISODate("2024-10-25T00:00:00Z"),
        DueDate: ISODate("2024-12-18T00:00:00Z"),
        Amount: NumberDecimal("80875.27"),
        Currency: "CNY",
        PaymentStatus: "Pending"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025010",
        IssueDate: ISODate("2025-09-21T00:00:00Z"),
        DueDate: ISODate("2025-10-15T00:00:00Z"),
        Amount: NumberDecimal("7024.28"),
        Currency: "AUD",
        PaymentStatus: "Pending"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024006",
        IssueDate: ISODate("2024-04-03T00:00:00Z"),
        DueDate: ISODate("2024-04-19T00:00:00Z"),
        Amount: NumberDecimal("30632.44"),
        Currency: "EUR",
        PaymentStatus: "Pending"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025012",
        IssueDate: ISODate("2025-10-24T00:00:00Z"),
        DueDate: ISODate("2025-11-04T00:00:00Z"),
        Amount: NumberDecimal("61696.31"),
        Currency: "INR",
        PaymentStatus: "Cancelled"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025013",
        IssueDate: ISODate("2025-01-07T00:00:00Z"),
        DueDate: ISODate("2025-01-26T00:00:00Z"),
        Amount: NumberDecimal("3216.18"),
        Currency: "AUD",
        PaymentStatus: "Partially Paid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025015",
        IssueDate: ISODate("2025-12-20T00:00:00Z"),
        DueDate: ISODate("2025-12-31T00:00:00Z"),
        Amount: NumberDecimal("100000.00"),
        Currency: "USD",
        PaymentStatus: "Paid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025014",
        IssueDate: ISODate("2025-07-11T00:00:00Z"),
        DueDate: ISODate("2025-07-18T00:00:00Z"),
        Amount: NumberDecimal("40805.19"),
        Currency: "JPY",
        PaymentStatus: "Pending"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025016",
        IssueDate: ISODate("2025-11-10T00:00:00Z"),
        DueDate: ISODate("2025-12-31T00:00:00Z"),
        Amount: NumberDecimal("44972.66"),
        Currency: "AUD",
        PaymentStatus: "Cancelled"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2024007",
        IssueDate: ISODate("2024-07-20T00:00:00Z"),
        DueDate: ISODate("2024-08-28T00:00:00Z"),
        Amount: NumberDecimal("42622.83"),
        Currency: "GBP",
        PaymentStatus: "Overdue"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025018",
        IssueDate: ISODate("2025-08-14T00:00:00Z"),
        DueDate: ISODate("2025-08-28T00:00:00Z"),
        Amount: NumberDecimal("50.00"),
        Currency: "USD",
        PaymentStatus: "Unpaid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025019",
        IssueDate: ISODate("2025-10-05T00:00:00Z"),
        DueDate: ISODate("2025-10-26T00:00:00Z"),
        Amount: NumberDecimal("79897.20"),
        Currency: "CNY",
        PaymentStatus: "Paid"
    },
    {
        _id: UUID(),
        InvoiceId: "INV2025020",
        IssueDate: ISODate("2025-06-29T00:00:00Z"),
        DueDate: ISODate("2025-07-22T00:00:00Z"),
        Amount: NumberDecimal("89556.04"),
        Currency: "CNY",
        PaymentStatus: "Paid"
    }
]);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All invoices are inserted successfully!");