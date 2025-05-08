using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common;
using Exadel.ReportHub.Data.Enums;
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
        user.Id = Guid.NewGuid();
        user.PasswordSalt = passwordSalt;
        user.PasswordHash = passwordHash;
        user.NotificationSettings = new NotificationSettings
        {
            ExportFormat = ExportFormat.Excel,
            Hour = Constants.Validation.NotificationSettings.DefaultHourValue,
            Frequency = NotificationFrequency.Weekly,
            DayOfWeek = DayOfWeek.Monday
        };

        await userRepository.AddAsync(user, cancellationToken);

        var userDto = mapper.Map<UserDTO>(user);
        return userDto;
    }
}
