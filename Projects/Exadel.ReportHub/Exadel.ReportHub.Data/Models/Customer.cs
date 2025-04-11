namespace Exadel.ReportHub.Data.Models;

public class Customer : IDocument
{
    public Guid Id { get; set; }

    public string Country { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }
}
