using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.GetProfile;

public record GetUserProfileRequest : IRequest<ErrorOr<UserProfileDTO>>;

public class GetUserProfileHandler(IUserRepository userRepository, IMapper mapper, IUserProvider userProvider) : IRequestHandler<GetUserProfileRequest, ErrorOr<UserProfileDTO>>
{
    public async Task<ErrorOr<UserProfileDTO>> Handle(GetUserProfileRequest request, CancellationToken cancellationToken)
    {
        var id = userProvider.GetUserId();
        var users = await userRepository.GetByIdAsync(id, cancellationToken);
        var userProfileDto = mapper.Map<UserProfileDTO>(users);
        return userProfileDto;
    }
}
