using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.GetById;

public record GetUserByIdRequest(Guid Id) : IRequest<ErrorOr<UserDTO>>;

public class GetUserByIdHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByIdRequest, ErrorOr<UserDTO>>
{
    public async Task<ErrorOr<UserDTO>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Error.NotFound();
        }

        var userDto = mapper.Map<UserDTO>(user);
        return userDto;
    }
}
