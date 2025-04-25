using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.AuditReport;
using MediatR;

namespace Exadel.ReportHub.Handlers.Audit.GetById;

public record GetAuditReportByIdRequest(Guid Id) : IRequest<ErrorOr<AuditReportDTO>>;

public class GetAuditReportByIdHandler(IAuditReportRepository auditReportRepository, IMapper mapper) : IRequestHandler<GetAuditReportByIdRequest, ErrorOr<AuditReportDTO>>
{
    public async Task<ErrorOr<AuditReportDTO>> Handle(GetAuditReportByIdRequest request, CancellationToken cancellationToken)
    {
        var auditReport = await auditReportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (auditReport is null)
        {
            return Error.NotFound();
        }

        return mapper.Map<AuditReportDTO>(auditReport);
    }
}
