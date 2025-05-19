using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.User.Create;
using Exadel.ReportHub.Handlers.User.Delete;
using Exadel.ReportHub.Handlers.User.Get;
using Exadel.ReportHub.Handlers.User.GetById;
using Exadel.ReportHub.Handlers.User.GetClients;
using Exadel.ReportHub.Handlers.User.GetProfile;
using Exadel.ReportHub.Handlers.User.UpdateActivity;
using Exadel.ReportHub.Handlers.User.UpdateName;
using Exadel.ReportHub.Handlers.User.UpdateNotificationSettings;
using Exadel.ReportHub.Handlers.User.UpdatePassword;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/users")]
public class UsersService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user", Description = "Creates a new user with the provided details.")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.User.Status201CreateDescription, typeof(ActionResult<UserDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] CreateUserDTO createUserDto)
    {
        var result = await sender.Send(new CreateUserRequest(createUserDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get user details", Description = "Retrieves the details of a user by their unique id.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.User.Status200RetrieveDescription, typeof(ActionResult<UserDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetUserByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get list of users", Description = "Retrieves a list of users, optionally filtered by their active status.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.User.Status200RetrieveDescription, typeof(ActionResult<IList<UserDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<UserDTO>>> GetUsers([FromQuery] bool? isActive)
    {
        var result = await sender.Send(new GetUsersRequest(isActive));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("profile")]
    [SwaggerOperation(Summary = "Get user profile", Description = "Retrieves the profile of a user by their unique id.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.User.Status200RetrieveDescription, typeof(ActionResult<UserProfileDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<UserProfileDTO>> GetUserProfileById()
    {
        var result = await sender.Send(new GetUserProfileRequest());
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPatch("{id:guid}/activity")]
    [SwaggerOperation(Summary = "Update user activity status", Description = "Updates the activity status of a user.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateUserActivity([FromRoute] Guid id, [FromBody] bool isActive)
    {
        var result = await sender.Send(new UpdateUserActivityRequest(id, isActive));

        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("password")]
    [SwaggerOperation(Summary = "Update user password", Description = "Updates the password of the currently authenticated user.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateUserPassword([FromBody] string password)
    {
        var result = await sender.Send(new UpdateUserPasswordRequest(password));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPatch("{id:guid}/fullname")]
    [SwaggerOperation(Summary = "Update user full name", Description = "Updates the full name of the user specified by id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateUserFullName([FromRoute] Guid id, [FromBody] string fullName)
    {
        var result = await sender.Send(new UpdateUserNameRequest(id, fullName));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete user", Description = "Deletes a user by their unique id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204DeleteDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteUserRequest(id));
        return FromResult(result);
    }

    [Authorize]
    [HttpPut("notification-settings")]
    [SwaggerOperation(Summary = "Update user notification settings", Description = "Updates the notification settings of the authorized user.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateUserNotificationSettings([FromBody] UpdateNotificationSettingsDTO updateNotificationSettingsDto)
    {
        var result = await sender.Send(new UpdateUserNotificationSettingsRequest(updateNotificationSettingsDto));
        return FromResult(result);
    }

    [Authorize]
    [HttpPut("notification-settings/turn-off")]
    [SwaggerOperation(Summary = "Turn user notifications off", Description = "Turns notifications off for the authorized user.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.User.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> TurnOffUserNotificationSettings()
    {
        var result = await sender.Send(new UpdateUserNotificationSettingsRequest(null));
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("clients")]
    [SwaggerOperation(Summary = "Get user clients", Description = "Gets list of all user clients")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<UserClientDTO>>> GetUserClients()
    {
        var result = await sender.Send(new GetUserClientsRequest());
        return FromResult(result);
    }
}
