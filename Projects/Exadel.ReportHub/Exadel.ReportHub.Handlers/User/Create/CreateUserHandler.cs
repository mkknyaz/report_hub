using System.Runtime.InteropServices;
using ErrorOr;
using Exadel.ReportHub.Common;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;

namespace Exadel.ReportHub.Handlers.User.Create;

public record CreateUserRequest(CreateUserDTO CreateUserDto) : IRequest<ErrorOr<UserDTO>>;

public class CreateUserHandler(IUserRepository userRepository) : IRequestHandler<CreateUserRequest, ErrorOr<UserDTO>>
{
    public async Task<ErrorOr<UserDTO>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var (passwordHash, passwordSalt) = PasswordHasher.CreatePasswordHash(request.CreateUserDto.Password);

        var user = new Data.Models.User
        {
            Id = Guid.NewGuid(),
            Email = request.CreateUserDto.Email,
            FullName = request.CreateUserDto.FullName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await userRepository.AddAsync(user, cancellationToken);

        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
