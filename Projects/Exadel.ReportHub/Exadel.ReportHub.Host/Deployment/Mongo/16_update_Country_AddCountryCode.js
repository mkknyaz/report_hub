const scriptName = "16_update_Country_AddCountryCode";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

const countryCodesList = [
    "PL",
    "US",
    "IT",
    "FR",
    "CZ",
    "BG",
    "JP",
    "DK",
    "GB",
    "HU",
    "RO",
    "SE",
    "CH",
    "IS",
    "NO",
    "TR",
    "AU",
    "BR",
    "CA",
    "CN",
    "HK",
    "ID",
    "IL",
    "IN"
];

const countries = db.Country.find({}, { _id: 1, Name: 1, CurrencyCode: 1 }).toArray();

const updates = countries.map(country => {

    let countryCode = country.CurrencyCode.substring(0, 2).toUpperCase();

    if (!countryCodesList.includes(countryCode)) {
        countryCode = country.Name.substring(0, 2).toUpperCase();
    }

    return {
        updateOne: {
            filter: { _id: country._id },
            update: {
                $set: { CountryCode: countryCode } }
        }
    };
});

if (updates.length > 0) {
    db.Country.bulkWrite(updates);
}

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Countries updated with Country Code based on Currency Code or Country.");
