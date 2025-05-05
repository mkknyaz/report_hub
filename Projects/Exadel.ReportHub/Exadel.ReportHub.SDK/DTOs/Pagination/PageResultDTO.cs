namespace Exadel.ReportHub.SDK.DTOs.Pagination;

public class PageResultDTO<T>
    where T : class
{
    public long Count { get; set; }

    public IList<T> Items { get; set; }
}
