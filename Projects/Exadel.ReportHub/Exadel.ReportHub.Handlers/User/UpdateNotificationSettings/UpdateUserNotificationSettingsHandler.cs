using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.UpdateNotificationFrequency;

public record UpdateUserNotificationSettingsRequest(NotificationSettingsDTO NotificationSettingsDTO) : IRequest<ErrorOr<Updated>>;

public class UpdateUserNotificationSettingsHandler(
    IUserRepository userRepository,
    IMapper mapper,
    IUserProvider userProvider) : IRequestHandler<UpdateUserNotificationSettingsRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserNotificationSettingsRequest request, CancellationToken cancellationToken)
    {
        var id = userProvider.GetUserId();
        var notificationSettings = mapper.Map<Data.Models.NotificationSettings>(request.NotificationSettingsDTO);
        await userRepository.UpdateNotificationSettingsAsync(id, notificationSettings, cancellationToken);
        return Result.Updated;
    }
}
