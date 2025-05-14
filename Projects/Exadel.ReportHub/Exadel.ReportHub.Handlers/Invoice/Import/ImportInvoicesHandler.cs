using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Import;

public record ImportInvoicesRequest(Guid ClientId, ImportDTO ImportDto) : IRequest<ErrorOr<ImportResultDTO>>;

public class ImportInvoicesHandler(
    ICsvImporter csvImporter,
    IInvoiceManager invoiceManager,
    IValidator<CreateInvoiceDTO> invoiceValidator,
    IMapper mapper) : IRequestHandler<ImportInvoicesRequest, ErrorOr<ImportResultDTO>>
{
    public async Task<ErrorOr<ImportResultDTO>> Handle(ImportInvoicesRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.ImportDto.File.OpenReadStream();

        var importInvoiceDtos = csvImporter.Read<ImportInvoiceDTO>(stream);
        var createInvoiceDtos = mapper.Map<List<CreateInvoiceDTO>>(importInvoiceDtos);
        foreach (var dto in createInvoiceDtos)
        {
            dto.ClientId = request.ClientId;
        }

        var validationTasks = createInvoiceDtos.Select(dto => invoiceValidator.ValidateAsync(dto, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);

        var validationErrors = validationResults
            .SelectMany((dto, index) => dto.Errors.Select(m => (RowIndex: index, Error: m.ErrorMessage)))
            .OrderBy(x => x.RowIndex)
            .ToList();

        if (validationErrors.Count > 0)
        {
            return validationErrors
                .Select(m => Error.Validation(description: $"Row {m.RowIndex + 1}: {m.Error}"))
                .ToList();
        }

        var invoiceDtos = await invoiceManager.CreateInvoicesAsync(createInvoiceDtos, cancellationToken);
        return new ImportResultDTO { ImportedCount = invoiceDtos.Count };
    }
}
