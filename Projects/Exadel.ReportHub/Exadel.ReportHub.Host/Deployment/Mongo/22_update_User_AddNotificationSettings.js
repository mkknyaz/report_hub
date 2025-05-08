const scriptName = "22_update_User_AddNotificationSettings";
const version = NumberInt(1);

if (db.MigrationHistory.findOne({ ScriptName: scriptName, Version: version })) {
    print(`${scriptName} v${version} is already applied`);
    quit();
}

const frequencies = ["Daily", "Weekly", "Monthly"];
const daysOfWeek = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
const exportFormats = ["Excel", "CSV"];
const hours = [8, 9, 10, 11, 14, 15, 16, 17];

const users = db.User.find({}, { _id: 1 }).toArray();

var updates = users.map(user => {
    const notificationSettings =
    {
        DayOfWeek: null,
        DayOfMonth: null,
    };
    const now = new Date();
    const randomFrequency = frequencies[Math.floor(Math.random() * frequencies.length)];
    notificationSettings.Frequency = randomFrequency;



    const reportHour = hours[Math.floor(Math.random() * hours.length)];
    notificationSettings.Hour = reportHour;

    const end = new Date(now);
    end.setHours(reportHour - 1, 0, 0, 0);

    const start = new Date(end);
    start.setDate(start.getDate() - 1);

    notificationSettings.ReportStartDate = start
    notificationSettings.ReportEndDate = end;


    if (randomFrequency === "Weekly") {

        const randomDay = daysOfWeek[Math.floor(Math.random() * daysOfWeek.length)]
        notificationSettings.DayOfWeek = randomDay;

        const targetDayIndex = daysOfWeek.indexOf(randomDay);
        const currentDayIndex = now.getDay();
        const daysSinceLast = (currentDayIndex - targetDayIndex + 7) % 7;

        const endDate = new Date(now);
        endDate.setDate(now.getDate() - daysSinceLast);
        endDate.setHours(reportHour - 1, 0, 0, 0);

        const startDate = new Date(endDate);
        startDate.setDate((endDate.getDate() - daysSinceLast) - 7);


        notificationSettings.ReportStartDate = startDate;
        notificationSettings.ReportEndDate = endDate;
    }
    else if (randomFrequency === "Monthly") {

        const randomDayOfMonth = Math.floor(Math.random() * 28) + 1;
        notificationSettings.DayOfMonth = randomDayOfMonth;

        const today = new Date();
        let endDate = new Date(today.getFullYear(), today.getMonth(), randomDayOfMonth, reportHour - 1, 0, 0, 0);
        if (endDate > today) {
            endDate = new Date(today.getFullYear(), today.getMonth() - 1, randomDayOfMonth, reportHour - 1, 0, 0, 0);
        }

        const startDate = new Date(endDate);
        startDate.setDate(startDate.getDate() - 30);

        notificationSettings.ReportStartDate = startDate;
        notificationSettings.ReportEndDate = endDate;
    }

    notificationSettings.ExportFormat = exportFormats[Math.floor(Math.random() * exportFormats.length)];

    return {
        updateOne: {
            filter: { _id: user._id },
            update: {
                $set: {
                    NotificationSettings: notificationSettings
                }
            }
        }
    };
});

if (updates.length > 0) {
    db.User.bulkWrite(updates);
}

db.MigrationHistory.insertOne({
    ScriptName: scriptName,
    Version: version,
    ScriptRunTime: new Date()
});

print("Users Updated Successfully");