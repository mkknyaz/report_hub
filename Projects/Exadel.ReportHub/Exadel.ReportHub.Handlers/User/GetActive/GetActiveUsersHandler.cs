using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.GetAllActive;

public record GetActiveUsersRequest : IRequest<ErrorOr<IEnumerable<UserDTO>>>;

public class GetActiveUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetActiveUsersRequest, ErrorOr<IEnumerable<UserDTO>>>
{
    public async Task<ErrorOr<IEnumerable<UserDTO>>> Handle(GetActiveUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllActiveAsync(cancellationToken);

        var userDtos = mapper.Map<List<UserDTO>>(users);

        return userDtos;
    }
}
