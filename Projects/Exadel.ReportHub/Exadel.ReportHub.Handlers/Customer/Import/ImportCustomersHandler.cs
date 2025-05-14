using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Import;

public record ImportCustomersRequest(Guid ClientId, ImportDTO ImportDto) : IRequest<ErrorOr<ImportResultDTO>>;

public class ImportCustomersHandler(
    IExcelImporter excelImporter,
    ICustomerManager customerManager,
    IValidator<ImportCustomerDTO> createCustomerValidator,
    IMapper mapper) : IRequestHandler<ImportCustomersRequest, ErrorOr<ImportResultDTO>>
{
    public async Task<ErrorOr<ImportResultDTO>> Handle(ImportCustomersRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.ImportDto.File.OpenReadStream();

        var importCustomerDtos = excelImporter.Read<ImportCustomerDTO>(stream);

        var validationTasks = importCustomerDtos.Select(dto => createCustomerValidator.ValidateAsync(dto, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);

        var validationErrors = validationResults
            .SelectMany((result, index) => result.Errors.Select(error => (RowIndex: index, Error: error.ErrorMessage)))
            .OrderBy(x => x.RowIndex)
            .ToList();

        if (validationErrors.Count > 0)
        {
            return validationErrors
                .Select(x => Error.Validation(description: $"Row {x.RowIndex + 1}: {x.Error}"))
                .ToList();
        }

        var createCustomerDtos = mapper.Map<List<CreateCustomerDTO>>(importCustomerDtos);
        foreach (var dto in createCustomerDtos)
        {
            dto.ClientId = request.ClientId;
        }

        var customerDtos = await customerManager.CreateCustomersAsync(createCustomerDtos, cancellationToken);

        return new ImportResultDTO { ImportedCount = customerDtos.Count };
    }
}
