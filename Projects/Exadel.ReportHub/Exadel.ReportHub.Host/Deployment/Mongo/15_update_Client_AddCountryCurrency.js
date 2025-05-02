const scriptName = "15_update_Client_AddCountryCurrency";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

const countries = db.Country.find().toArray();

function getCountryData(bankAccountNumber) {
    if (!bankAccountNumber) {
        return null;
    }

    const countryCode = bankAccountNumber.substring(0, 2).toUpperCase();

    const countryInfo = countries.find(country => country.CountryCode.toUpperCase() === countryCode.toUpperCase());

    return countries.find(x => x.Name === countryInfo.Name);
}
const clients = db.Client.find({}, { _id: 1, BankAccountNumber: 1 }).toArray();

const updates = clients.map(client => {
    const countryData = getCountryData(client.BankAccountNumber);

    const update = {
        CountryId: null,
        CountryName: null,
        CurrencyId: null,
        CurrencyCode: null
    };
    if (countryData) {
        update.CountryId = countryData._id;
        update.CountryName = countryData.Name;
        update.CurrencyId = countryData.CurrencyId;
        update.CurrencyCode = countryData.CurrencyCode;
    }

    return {
        updateOne: {
            filter: { _id: client._id },
            update: { $set: update }
        }
    };
});

if (updates.length > 0) {
    db.Client.bulkWrite(updates);
}

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Clients updated with country and currency based on bank account number.");
