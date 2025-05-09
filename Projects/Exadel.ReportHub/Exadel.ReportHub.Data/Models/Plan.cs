using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Plan : IDocument, ISoftDeletable
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid ItemId { get; set; }

    public int Count { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsDeleted { get; set; }
}
