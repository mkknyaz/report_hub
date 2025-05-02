namespace Exadel.ReportHub.SDK.DTOs.Plan;

public class CreatePlanDTO : UpdatePlanDTO
{
    public Guid ClientId { get; set; }

    public Guid ItemId { get; set; }
}
