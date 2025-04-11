const scriptName = "03_create_User";
const version = NumberInt(2);

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

const userNames = [
    "John Travolta",
    "Jim Carrey",
    "Benedict Cumberbatch",
    "Hugh Jackman",
    "Johnny Depp",
    "Kevin Spacey",
    "Denzel Washington",
    "Brad Pitt",
    "Angelina Jolie",
    "Tom Cruise"
]

// Password is user first name
const passwords = [
    {
        Hash: "JOAo5O8oruDQf2B4VQ1V6LMKYK0JWDy7fNZB4XB7GVDTYOfoD0wxud9LfFyaQM2U+Ni/7clgF8+eA/5KzUMdbg==",
        Salt: "9ZA4jp5J/78Y8XJcmyAj8Q=="
    },
    {
        Hash: "EwfrTotuZPeNmyvGrp3wSDu8p+Bj5OvcpebFSgFWSNPZLNUcYZr7qJpoNHPoKjsnN2xgdUVR9cGAUFTc1iXZ/w==",
        Salt: "8GVmvIHlfxXte64+lw71RA=="
    },
    {
        Hash: "W1aj7oWttdVt4fnLXXEX4Ei0z4Wq+qEzgRWAhNLX20DZoYvvj0b/6lRHztNFoOj5c8+UfseUpE+oeUD2D5tIsQ==",
        Salt: "Ies/mNpuwqXB1/39gdzgyw=="
    },
    {
        Hash: "+rH+5kYGVn9bpb9Nywqa6BIipIEFzzOIn+TC2os/iVcWtTOMA+UGStBp7LihnV1lGDUZ6mwmqSARFJ1P9XSd2A==",
        Salt: "yD8RG0O2iiaxEJh5Jwktcg=="
    },
    {
        Hash: "+OsKb1xrQXNNv9kOIQCY0e9ZUa/mClrhWmHxP/aL7Li4UnDUN2sYSq1vPtF9BkPsSq2yaklkuQdGh8IiLJjQxA==",
        Salt: "4tciCCYpdvBfVtWs8qoYFA=="
    },
    {
        Hash: "u8W7cysuLKDV40tig5xiqBNlMmA2zaVZD/LmxKgxZXHlg+J9GF6oIvKMkSpc4PNGKR8tGoqEx1rzC0JGbz402A==",
        Salt: "iTZYzaBFE2hi4EDij+0z3Q=="
    },
    {
        Hash: "noHrtHxjdi3MwpPfJ78bm4kvXeAh+acAtO6v5RnGrBsU9RovTVL4KHdydoQcB8T6jki9kUKrVzP+HHbbzaFH+g==",
        Salt: "XbCky1tvZvlseEZ/uOmVJw=="
    },
    {
        Hash: "leocdbJC+8Ua5EPyzlWRlxE0bcOsk0/o6TcAusp0F1qKCCM9XPbB09RUpdYPXG19pYt9adX0xAVlKbd9aJQJGQ==",
        Salt: "HFYW9AiKAFkGNJXn4Su8Lw=="
    },
    {
        Hash: "3xLeaaFkqNoeFzWxvJLGA1Ql/Hk34DocFDIPl8j4rPv33Ghyn38JTXeXm6W9loz7hoMc0W2veM0an0pJut3bQQ==",
        Salt: "3fzUHnYsvpTzblX5aAWvGA=="
    },
    {
        Hash: "nuJMGIWnR2IGgWxo9vd2Jf4ksFD6Toe9smt+lkjLUq2LReji80HgQGOj87Y/NAoc4KklBr6UnbHTAwTE/HyAzg==",
        Salt: "b7nkA/NDk65Uvw8SI97ksg=="
    }
]

const users = [];
const userCount = 10;

for (let i = 0; i < userCount; i++) {
    users.push({
        _id: userIds[i],
        Email: userNames[i].replace(/\s/g, '').toLowerCase() + "@test.com",
        FullName: userNames[i],
        PasswordHash: passwords[i].Hash,
        PasswordSalt: passwords[i].Salt,
        IsActive: true
    });
}

const opt = users.map(user => ({
    replaceOne: {
        filter: { _id: user._id },
        replacement: user,
        upsert: true
    }
}));

db.User.bulkWrite(opt);

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("All users are inserted successfully!");