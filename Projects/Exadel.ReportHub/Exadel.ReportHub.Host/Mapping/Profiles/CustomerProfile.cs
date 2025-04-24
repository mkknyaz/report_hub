using AutoMapper;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.SDK.DTOs.Customer;

namespace Exadel.ReportHub.Host.Mapping.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CreateCustomerDTO, Customer>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Country, opt => opt.Ignore())
            .ForMember(x => x.CurrencyId, opt => opt.Ignore())
            .ForMember(x => x.CurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore());
        CreateMap<Customer, CustomerDTO>();
        CreateMap<UpdateCustomerDTO, Customer>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Email, opt => opt.Ignore())
            .ForMember(x => x.Country, opt => opt.Ignore())
            .ForMember(x => x.CurrencyId, opt => opt.Ignore())
            .ForMember(x => x.CurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore());
    }
}
