using System.Security;
using ErrorOr;
using Exadel.ReportHub.Common;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.UpdatePassword;

public record UpdateUserPasswordRequest(string Password) : IRequest<ErrorOr<Updated>>;

public class UpdateUserPasswordHandler(IUserRepository userRepository, IUserProvider userProvider) : IRequestHandler<UpdateUserPasswordRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserPasswordRequest request, CancellationToken cancellationToken)
    {
        (string passwordHash, string passwordSalt) = PasswordHasher.CreatePasswordHash(request.Password);
        var userId = userProvider.GetUserId();
        await userRepository.UpdatePasswordAsync(userId, passwordHash, passwordSalt, cancellationToken);
        return Result.Updated;
    }
}
