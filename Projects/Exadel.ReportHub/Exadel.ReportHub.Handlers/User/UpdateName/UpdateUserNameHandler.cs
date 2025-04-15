using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.UpdateName;

public record UpdateUserNameRequest(Guid Id, string FullName) : IRequest<ErrorOr<Updated>>;

public class UpdateUserNameHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserNameRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserNameRequest request, CancellationToken cancellationToken)
    {
        var isExists = await userRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await userRepository.UpdateNameAsync(request.Id, request.FullName, cancellationToken);

        return Result.Updated;
    }
}
