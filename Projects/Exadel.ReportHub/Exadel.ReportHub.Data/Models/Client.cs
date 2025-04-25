using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Client : IDocument, ISoftDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string BankAccountNumber { get; set; }

    public bool IsDeleted { get; set; }
}
