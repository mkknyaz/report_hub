using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.GetClients;

public record GetUserClientsRequest : IRequest<ErrorOr<IList<UserClientDTO>>>;

public class GetUserClientsHandler(
    IUserProvider userProvider,
    IClientRepository clientRepository,
    IUserAssignmentRepository userAssignmentRepository) : IRequestHandler<GetUserClientsRequest, ErrorOr<IList<UserClientDTO>>>
{
    public async Task<ErrorOr<IList<UserClientDTO>>> Handle(GetUserClientsRequest request, CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();

        var userAssignments = await userAssignmentRepository.GetUserAssignmentsAsync(userId, cancellationToken);
        if (userAssignments is null || userAssignments.Count == 0)
        {
            return new List<UserClientDTO>();
        }

        var clientIds = userAssignments.Select(x => x.ClientId).ToList();
        var clients = await clientRepository.GetByIdsAsync(clientIds, cancellationToken);

        var clientLookup = clients.ToDictionary(c => c.Id, c => c);

        var result = userAssignments
            .Where(a => clientLookup.ContainsKey(a.ClientId))
            .Select(a =>
            {
                var client = clientLookup[a.ClientId];
                return new UserClientDTO
                {
                    ClientId = client.Id,
                    ClientName = client.Name,
                    Role = (SDK.Enums.UserRole)a.Role
                };
            })
            .ToList();

        return result;
    }
}
