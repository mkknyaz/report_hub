using ErrorOr;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Handlers.User.UpdateActivity;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.UpdateRole;

public record UpdateUserRoleRequest(Guid Id, UserRole Role) : IRequest<ErrorOr<Updated>>;

public class UpdateUserRoleHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserRoleRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        var isExists = await userRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await userRepository.UpdateRoleAsync(request.Id, request.Role, cancellationToken);
        return Result.Updated;
    }
}
