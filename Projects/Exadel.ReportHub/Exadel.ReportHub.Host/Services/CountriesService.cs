using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Country.GetAll;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Country;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/countries")]
public class CountriesService(ISender sender) : BaseService
{
    [Authorize]
    [HttpGet]
    [SwaggerOperation(Summary = "Get all countries", Description = "Returns a list of all available countries")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Country.Status200RetrieveDescription, typeof(ActionResult<IList<CountryDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<CountryDTO>>> GetAllCountries()
    {
        var result = await sender.Send(new GetAllCountriesRequest());

        return FromResult(result);
    }
}
