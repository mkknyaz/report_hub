using AutoMapper;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.SDK.DTOs.UserAssignment;

namespace Exadel.ReportHub.Host.Mapping.Profiles;

public class UserAssignmentProfile : Profile
{
    public UserAssignmentProfile()
    {
        CreateMap<UpsertUserAssignmentDTO, UserAssignment>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}
