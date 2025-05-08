using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Send;

public record SendReportsRequest : IRequest<Unit>;

public class SendReportsHandler(IUserRepository userRepository) : IRequestHandler<SendReportsRequest, Unit>
{
    public async Task<Unit> Handle(SendReportsRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentHour = now.Hour;
        var currentDay = now.Day;
        var currentDayOfWeek = now.DayOfWeek;
        var users = await userRepository.GetUsersByNotificationSettingsAsync(currentDay, currentDayOfWeek, currentHour, cancellationToken);

        return Unit.Value;
    }
}
