const scriptName = "19_update_Invoice_CustomerAndClientCurrencyFields";
const version = NumberInt(2);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);;
    quit();
}

const invoices = db.Invoice.find({}).toArray();

const clientIds = [...new Set(invoices.map(invoice => invoice.ClientId))];
const clients = db.Client.find({ _id: { $in: clientIds } }, { _id: 1, CurrencyId: 1, CurrencyCode: 1 }).toArray();

const clientsDict = clients.reduce((key, client) => {
    key[client._id] = {
        CurrencyId: client.CurrencyId,
        CurrencyCode: client.CurrencyCode
    };
    return key;
}, {});

const dates = db.Invoice.aggregate([
    {
        $group: {
            _id: null,
            minDate: { $min: "$IssueDate" },
            maxDate: { $max: "$IssueDate" }
        }
    }
]).toArray()[0];

const exchangeRates = db.ExchangeRate.find({
    RateDate: {
        $gte: dates.minDate,
        $lte: dates.maxDate
    }
}).toArray();

const exchangeRatesDict = exchangeRates.reduce((key, rate) => {
    if (!key[rate.Currency]) {
        key[rate.Currency] = [];
    }
    key[rate.Currency].push(rate);
    return key;
}, {});

db.ExchangeRate.createIndex({ RateDate: 1 });

const updates = invoices.map(invoice => {

    const client = clientsDict[invoice.ClientId];

    const ratesForCurrency = exchangeRatesDict[client.CurrencyCode] || [];


    const update = {
        CustomerCurrencyAmount: invoice.Amount,
        CustomerCurrencyId: invoice.CurrencyId,
        CustomerCurrencyCode: invoice.CurrencyCode,

        ClientCurrencyAmount: 0,
        ClientCurrencyId: client.CurrencyId,
        ClientCurrencyCode: client.CurrencyCode,
    };
    if (invoice.CurrencyCode === client.CurrencyCode) {
        update.ClientCurrencyAmount = invoice.Amount;
    }
    else {
        const clientCurrencyRates = ratesForCurrency.filter(rate =>
            new Date(rate.RateDate) <= new Date(invoice.IssueDate));

        if (clientCurrencyRates.length > 0) {
            const maxDate = new Date(Math.max(...clientCurrencyRates.map(x =>
                new Date(x.RateDate).getTime()
            )));

            const closestRate = clientCurrencyRates.find(x =>
                new Date(x.RateDate).getTime() === maxDate.getTime()
            );

            clientCurrencyAmount = invoice.Amount * closestRate.Rate;
            update.ClientCurrencyAmount = NumberDecimal(clientCurrencyAmount.toFixed(4));
        }

    }

    return {
        updateOne: {
            filter: { _id: invoice._id },
            update: {
                $set: update,
                $unset: {
                    Amount: "",
                    CurrencyId: "",
                    CurrencyCode: ""
                }
            }
        }
    };
});

if (updates.length > 0) {
    db.Invoice.bulkWrite(updates);
}

db.ExchangeRate.dropIndex({ RateDate: 1 });

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Invoices updated with Client Currency and Customer Currency fields");