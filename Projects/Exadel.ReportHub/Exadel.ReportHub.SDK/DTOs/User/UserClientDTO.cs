using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.User;

public class UserClientDTO
{
    public Guid ClientId { get; set; }

    public string ClientName { get; set; }

    public UserRole Role { get; set; }
}
