using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Country : IDocument
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; }

    public string CountryCode { get; set; }
}
