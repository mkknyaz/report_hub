namespace Exadel.ReportHub.SDK.DTOs.Plan;

public class PlanDTO : UpdatePlanDTO
{
    public Guid Id { get; set; }

    public Guid ClientId { get; set; }

    public Guid ItemId { get; set; }
}
