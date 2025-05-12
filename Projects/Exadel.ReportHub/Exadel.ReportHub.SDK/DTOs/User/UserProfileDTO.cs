namespace Exadel.ReportHub.SDK.DTOs.User;

public class UserProfileDTO
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public NotificationSettingsDTO NotificationSettings { get; set; }
}
