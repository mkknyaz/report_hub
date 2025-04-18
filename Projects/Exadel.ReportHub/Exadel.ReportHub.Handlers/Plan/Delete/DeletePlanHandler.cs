using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Plan.Delete;

public record DeletePlanRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeletePlanHandler(IPlanRepository planRepository) : IRequestHandler<DeletePlanRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeletePlanRequest request, CancellationToken cancellationToken)
    {
        var isExists = await planRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await planRepository.SoftDeleteAsync(request.Id, cancellationToken);
        return Result.Deleted;
    }
}
