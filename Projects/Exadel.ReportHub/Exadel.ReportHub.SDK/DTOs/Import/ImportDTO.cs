using Microsoft.AspNetCore.Http;

namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class ImportDTO
{
    public IFormFile File { get; set; }
}
