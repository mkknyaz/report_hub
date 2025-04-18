using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using MediatR;

namespace Exadel.ReportHub.Handlers.Plan.Update;

public record UpdatePlanRequest(Guid Id, UpdatePlanDTO UpdatePlanDatedto) : IRequest<ErrorOr<Updated>>;

public class UpdatePlanHandler(IPlanRepository planRepository, IMapper mapper) : IRequestHandler<UpdatePlanRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdatePlanRequest request, CancellationToken cancellationToken)
    {
        var isExists = await planRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        var plan = mapper.Map<Data.Models.Plan>(request.UpdatePlanDatedto);
        await planRepository.UpdateDateAsync(request.Id, plan, cancellationToken);
        return Result.Updated;
    }
}
