using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Client : IDocument, ISoftDeletable, ICountryBasedDocument
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string BankAccountNumber { get; set; }

    public Guid CountryId { get; set; }

    public string Country { get; set; }

    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; }

    public bool IsDeleted { get; set; }
}
