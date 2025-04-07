const scriptName = "03_create_User";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

db.createCollection("User", {
    collation: {
        locale: "en"
    }
});

db.User.createIndex(
    { Email: 1 },
    {
        unique: true,
        background: true
    });

db.User.insertMany([
    {
        _id: UUID(),
        Email: "demo.user1@gmail.com",
        FullName: "Tony Stark",
        // Pa$$word
        PasswordHash: "fA0ifIwA6cePXixAIKglM/WYky9Eg5cf0rTQM/i4VPCweTEFZlaRFXbNbze7AxCg8ph8bVGaby9ja8Jv1JGtAw==",
        PasswordSalt: "g2X1ISQcfqmdvtxH1QEeww==",
        Role: "Regular",
        IsActive: false
    },
    {
        _id: UUID(),
        Email: "demo.user2@gmail.com",
        FullName: "Jim Carrey",
        // Dem0Pass
        PasswordHash: "MCX7P74ysH4lGSFFnjSxLgG85yClViLoqExm9zuFbtPW2GivIYzg7UsnLO6AsGpjX9iWHSc9VYotfWYcglvxqg==",
        PasswordSalt: "X7VVi0KB03QUOV422OGouw==",
        Role: "Admin",
        IsActive: false
    },
    {
        _id: UUID(),
        Email: "demo.user3@gmail.com",
        FullName: "Benedict Cumberbatch",
        // UserPaSS
        PasswordHash: "cMwDkUtR/0q+WTZSTju7zncdBn6u3vjpJQsPZPSkF7M0dGKrD3HsTE7hIhHNKuypdm78/ycGv1hfXp8VCnVpww==",
        PasswordSalt: "IvV1bQJQHmVgc38aUvYG4A==",
        Role: "Regular",
        IsActive: true
    },
    {
        _id: UUID(),
        Email: "demo.user4@gmail.com",
        FullName: "Hugh Jackman",
        // AdminPa$$
        PasswordHash: "ILbBFLxfZi+Vn6/vXyiDOK8dQrdSMAKJMpZPEdQwex9HH2Xy1e84yk4FJLufxf76YqerG8yoPWKegK+/qY/PfQ==",
        PasswordSalt: "7XRYciM8IydOwzaUWbYo2Q==",
        Role: "Admin",
        IsActive: true
    }
]);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All users are inserted successfully!");