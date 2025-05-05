using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.AuditReport;
using Exadel.ReportHub.SDK.DTOs.Pagination;
using MediatR;

namespace Exadel.ReportHub.Handlers.Audit.GetByUserId;

public record GetAuditReportsByUserIdRequest(Guid UserId, PageRequestDTO PageRequestDto) : IRequest<ErrorOr<PageResultDTO<AuditReportDTO>>>;

public class GetAuditReportsByUserIdHandler(
    IAuditReportRepository auditReportRepository,
    IMapper mapper) : IRequestHandler<GetAuditReportsByUserIdRequest, ErrorOr<PageResultDTO<AuditReportDTO>>>
{
    public async Task<ErrorOr<PageResultDTO<AuditReportDTO>>> Handle(GetAuditReportsByUserIdRequest request, CancellationToken cancellationToken)
    {
        var count = await auditReportRepository.GetCountAsync(request.UserId, cancellationToken);

        request.PageRequestDto.Top = request.PageRequestDto.Top == 0 ?
            Constants.Validation.Pagination.MaxValue : request.PageRequestDto.Top;

        var auditReports = await auditReportRepository.GetByUserIdAsync(request.UserId,
            request.PageRequestDto.Skip, request.PageRequestDto.Top, cancellationToken);

        var pageResult = new PageResultDTO<AuditReportDTO>
        {
            Count = count,
            Items = mapper.Map<List<AuditReportDTO>>(auditReports),
        };
        return pageResult;
    }
}
