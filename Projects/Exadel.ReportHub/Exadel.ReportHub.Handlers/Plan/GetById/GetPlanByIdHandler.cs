using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using MediatR;

namespace Exadel.ReportHub.Handlers.Plan.GetById;

public record GetPlanByIdRequest(Guid Id) : IRequest<ErrorOr<PlanDTO>>;

public class GetPlanByIdHandler(IPlanRepository planRepository, IMapper mapper) : IRequestHandler<GetPlanByIdRequest, ErrorOr<PlanDTO>>
{
    public async Task<ErrorOr<PlanDTO>> Handle(GetPlanByIdRequest request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plan == null)
        {
            return Error.NotFound();
        }

        var planDto = mapper.Map<PlanDTO>(plan);
        return planDto;
    }
}
