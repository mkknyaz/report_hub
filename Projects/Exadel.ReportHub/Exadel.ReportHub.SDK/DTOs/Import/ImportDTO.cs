using Microsoft.AspNetCore.Http;

namespace Exadel.ReportHub.SDK.DTOs.Import;

public class ImportDTO
{
    public IFormFile File { get; set; }
}
