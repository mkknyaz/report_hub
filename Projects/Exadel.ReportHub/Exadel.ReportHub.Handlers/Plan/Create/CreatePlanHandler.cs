using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using MediatR;

namespace Exadel.ReportHub.Handlers.Plan.Create;

public record CreatePlanRequest(CreatePlanDTO CreatePlanDto) : IRequest<ErrorOr<PlanDTO>>;
public class CreatePlanHandler(IPlanRepository planRepository, IMapper mapper) : IRequestHandler<CreatePlanRequest, ErrorOr<PlanDTO>>
{
    public async Task<ErrorOr<PlanDTO>> Handle(CreatePlanRequest request, CancellationToken cancellationToken)
    {
        var plan = mapper.Map<Data.Models.Plan>(request.CreatePlanDto);
        plan.Id = Guid.NewGuid();

        await planRepository.AddAsync(plan, cancellationToken);
        return mapper.Map<PlanDTO>(plan);
    }
}
