using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class Report : IDocument
{
    public Guid Id { get; set; }
}
