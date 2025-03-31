db.MigrationHistory.createIndex(
    { ScriptName: 1, Version: 1 },
    {
        unique: true,
        background: true
    });