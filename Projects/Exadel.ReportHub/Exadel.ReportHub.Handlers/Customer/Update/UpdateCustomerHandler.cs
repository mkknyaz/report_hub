using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Update;

public record UpdateCustomerRequest(Guid CustomerId, Guid ClientId, UpdateCustomerDTO UpdateCustomerDto) : IRequest<ErrorOr<Updated>>;

public class UpdateCustomerHandler(ICustomerRepository customerRepository, ICountryRepository countryRepository, IMapper mapper) : IRequestHandler<UpdateCustomerRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var isExists = await customerRepository.ExistsAsync(request.CustomerId, request.ClientId, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        var customer = mapper.Map<Data.Models.Customer>(request.UpdateCustomerDto);
        var country = await countryRepository.GetByIdAsync(request.UpdateCustomerDto.CountryId, cancellationToken);

        customer.Id = request.CustomerId;
        customer.Country = country.Name;
        customer.CurrencyId = country.CurrencyId;
        customer.CurrencyCode = country.CurrencyCode;

        await customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Updated;
    }
}
