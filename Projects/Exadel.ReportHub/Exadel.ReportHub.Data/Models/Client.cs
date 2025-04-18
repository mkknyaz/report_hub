using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Client : IDocument, ISoftDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public IList<Guid> CustomerIds { get; set; }

    public bool IsDeleted { get; set; }
}
