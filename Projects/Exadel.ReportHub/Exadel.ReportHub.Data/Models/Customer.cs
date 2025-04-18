using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Customer : IDocument, ISoftDeletable
{
    public Guid Id { get; set; }

    public string Country { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }
}
