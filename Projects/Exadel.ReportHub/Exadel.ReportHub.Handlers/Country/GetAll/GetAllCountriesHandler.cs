using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Country;
using MediatR;

namespace Exadel.ReportHub.Handlers.Country.GetAll;

public record GetAllCountriesRequest : IRequest<ErrorOr<IList<CountryDTO>>>;

public class GetAllCountriesHandler(ICountryRepository countryRepository, IMapper mapper) : IRequestHandler<GetAllCountriesRequest, ErrorOr<IList<CountryDTO>>>
{
    public async Task<ErrorOr<IList<CountryDTO>>> Handle(GetAllCountriesRequest request, CancellationToken cancellationToken)
    {
        return mapper.Map<List<CountryDTO>>(await countryRepository.GetAllAsync(cancellationToken));
    }
}
