using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Import;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Handlers.Client.Import;

public record ImportClientRequest(ImportDTO ImportDTO) : IRequest<ErrorOr<ImportResultDTO>>;

public class ImportClientsHandler(
    IExcelImporter excelImporter,
    IClientRepository clientRepository,
    ICountryBasedEntityManager countryBasedEntityManager,
    IValidator<CreateClientDTO> createClientValidator) : IRequestHandler<ImportClientRequest, ErrorOr<ImportResultDTO>>
{
    public async Task<ErrorOr<ImportResultDTO>> Handle(ImportClientRequest request, CancellationToken cancellationToken)
    {
        using var stream = request.ImportDTO.File.OpenReadStream();

        var clientDtos = excelImporter.Read<CreateClientDTO>(stream);

        var tasks = clientDtos.Select(dto => createClientValidator.ValidateAsync(dto, cancellationToken));
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

        var clients = await countryBasedEntityManager.GenerateEntitiesAsync<CreateClientDTO, Data.Models.Client>(clientDtos, cancellationToken);

        await clientRepository.AddManyAsync(clients, cancellationToken);

        return new ImportResultDTO { ImportedCount = clients.Count };
    }
}
