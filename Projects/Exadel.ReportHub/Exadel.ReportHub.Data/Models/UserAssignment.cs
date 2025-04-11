using Exadel.ReportHub.Data.Enums;

namespace Exadel.ReportHub.Data.Models;

public class UserAssignment : IDocument
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ClientId { get; set; }

    public UserRole Role { get; set; }
}
