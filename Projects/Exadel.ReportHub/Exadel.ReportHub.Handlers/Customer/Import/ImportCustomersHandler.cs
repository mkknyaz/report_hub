using ErrorOr;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Import;

public record ImportCustomersRequest(ImportDTO ImportDTO) : IRequest<ErrorOr<ImportResultDTO>>;

public class ImportCustomersHandler(
    IExcelImporter excelImporter,
    ICustomerRepository customerRepository,
    IValidator<CreateCustomerDTO> createCustomerValidator,
    ICountryBasedEntityManager countryBasedEntityManager) : IRequestHandler<ImportCustomersRequest, ErrorOr<ImportResultDTO>>
{
    public async Task<ErrorOr<ImportResultDTO>> Handle(ImportCustomersRequest request, CancellationToken cancellationToken)
    {
        using var stream = request.ImportDTO.File.OpenReadStream();

        var customerDtos = excelImporter.Read<CreateCustomerDTO>(stream);

        var tasks = customerDtos.Select(dto => createCustomerValidator.ValidateAsync(dto, cancellationToken));
        var validationResults = await Task.WhenAll(tasks);

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

        var customers = await countryBasedEntityManager.GenerateEntitiesAsync<CreateCustomerDTO, Data.Models.Customer>(customerDtos, cancellationToken);
        await customerRepository.AddManyAsync(customers, cancellationToken);

        return new ImportResultDTO { ImportedCount = customers.Count };
    }
}
