using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Currency : IDocument
{
    public Guid Id { get; set; }

    public string CurrencyCode { get; set; }
}
