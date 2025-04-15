using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.Delete;

public record DeleteUserRequest(Guid UserId) : IRequest<ErrorOr<Deleted>>;

public class DeleteUserHandler(IUserRepository userRepository, IUserAssignmentRepository userAssignmentRepository) : IRequestHandler<DeleteUserRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var userExists = await userRepository.ExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
        {
            return Error.NotFound();
        }

        var clientIds = await userAssignmentRepository.GetClientIdsAsync(request.UserId, cancellationToken);

        if (clientIds.Any())
        {
            await userAssignmentRepository.DeleteAsync(request.UserId, clientIds, cancellationToken);
        }

        await userRepository.DeleteAsync(request.UserId, cancellationToken);

        return Result.Deleted;
    }
}
