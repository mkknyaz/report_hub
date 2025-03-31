using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.GetAllActive;

public record GetActiveUsersRequest : IRequest<ErrorOr<IEnumerable<UserDTO>>>;

public class GetActiveUsersHandler(IUserRepository userRepository) : IRequestHandler<GetActiveUsersRequest, ErrorOr<IEnumerable<UserDTO>>>
{
    public async Task<ErrorOr<IEnumerable<UserDTO>>> Handle(GetActiveUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllActiveAsync(cancellationToken);
        var userDtos = users.Select(u => new UserDTO
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName
        }).ToList();

        return userDtos;
    }
}
