namespace Exadel.ReportHub.Ecb.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetWeekPeriodStart(this DateTime endDate)
    {
        const int daysToWeekStartCount = 6;
        return endDate.AddDays(-daysToWeekStartCount);
    }
}
