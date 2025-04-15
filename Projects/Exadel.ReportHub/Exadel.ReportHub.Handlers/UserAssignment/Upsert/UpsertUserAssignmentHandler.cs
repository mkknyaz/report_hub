using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.UserAssignment;
using MediatR;

namespace Exadel.ReportHub.Handlers.UserAssignment.Upsert;

public record UpsertUserAssignmentRequest(UpsertUserAssignmentDTO SetUserAssignmentDTO) : IRequest<ErrorOr<Updated>>;

public class UpsertUserAssignmentHandler(IUserAssignmentRepository userAssignmentRepository, IMapper mapper) : IRequestHandler<UpsertUserAssignmentRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpsertUserAssignmentRequest request, CancellationToken cancellationToken)
    {
        var userAssignment = mapper.Map<Data.Models.UserAssignment>(request.SetUserAssignmentDTO);

        await userAssignmentRepository.UpsertAsync(userAssignment, cancellationToken);
        return Result.Updated;
    }
}
