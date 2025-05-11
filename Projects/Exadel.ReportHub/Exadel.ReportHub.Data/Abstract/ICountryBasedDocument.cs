namespace Exadel.ReportHub.Data.Abstract;

public interface ICountryBasedDocument
{
    Guid CountryId { get; set; }

    string Country { get; set; }

    Guid CurrencyId { get; set; }

    string CurrencyCode { get; set; }
}
