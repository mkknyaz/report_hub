using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Country.GetAll;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Country;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/countries")]
public class CountriesService(ISender sender) : BaseService
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IList<CountryDTO>>> GetAllCountries()
    {
        var result = await sender.Send(new GetAllCountriesRequest());

        return FromResult(result);
    }
}
