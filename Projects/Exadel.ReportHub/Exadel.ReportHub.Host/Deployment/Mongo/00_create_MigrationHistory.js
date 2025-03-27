db.MigrationHistory.createIndex(
    { scriptName: 1, version: 1 },
    {
        unique: true,
        background: true
    });