using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.UpdateNotificationSettings;

public record UpdateUserNotificationSettingsRequest(UpdateNotificationSettingsDTO UpdateNotificationSettingsDto) : IRequest<ErrorOr<Updated>>;

public class UpdateUserNotificationSettingsHandler(
    IUserRepository userRepository,
    IClientRepository clientRepository,
    IMapper mapper,
    IUserProvider userProvider) : IRequestHandler<UpdateUserNotificationSettingsRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserNotificationSettingsRequest request, CancellationToken cancellationToken)
    {
        var id = userProvider.GetUserId();
        var notificationSettings = mapper.Map<Data.Models.NotificationSettings>(request.UpdateNotificationSettingsDto);
        if (notificationSettings is not null)
        {
            notificationSettings.ClientName = await clientRepository.GetNameAsync(notificationSettings.ClientId, cancellationToken);
        }

        await userRepository.UpdateNotificationSettingsAsync(id, notificationSettings, cancellationToken);
        return Result.Updated;
    }
}
