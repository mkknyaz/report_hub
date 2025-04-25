using AutoMapper;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.SDK.DTOs.AuditReport;

namespace Exadel.ReportHub.Host.Mapping.Profiles;

public class AuditReportProfile : Profile
{
    public AuditReportProfile()
    {
        CreateMap<AuditReport, AuditReportDTO>();
        CreateMap<IAuditAction, AuditReport>()
            .ForMember(x => x.Id, opt => opt.Ignore());
    }
}
