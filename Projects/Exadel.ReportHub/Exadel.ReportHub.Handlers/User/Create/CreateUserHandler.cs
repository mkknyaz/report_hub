using System.Runtime.InteropServices;
using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.Create;

public record CreateUserRequest(CreateUserDTO CreateUserDto) : IRequest<ErrorOr<UserDTO>>;

public class CreateUserHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<CreateUserRequest, ErrorOr<UserDTO>>
{
    public async Task<ErrorOr<UserDTO>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var (passwordHash, passwordSalt) = PasswordHasher.CreatePasswordHash(request.CreateUserDto.Password);

        var user = mapper.Map<Data.Models.User>(request.CreateUserDto);
        user.PasswordSalt = passwordSalt;
        user.PasswordHash = passwordHash;

        await userRepository.AddAsync(user, cancellationToken);
        var userDto = mapper.Map<UserDTO>(user);
        return userDto;
    }
}
