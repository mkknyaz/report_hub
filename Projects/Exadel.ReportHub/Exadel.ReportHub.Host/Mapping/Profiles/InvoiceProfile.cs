using AutoMapper;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Host.Mapping.Profiles;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<CreateInvoiceDTO, Invoice>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyAmount, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyId, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyAmount, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyId, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.ClientBankAccountNumber, opt => opt.Ignore())
            .ForMember(x => x.PaymentStatus, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceDTO>();

        CreateMap<UpdateInvoiceDTO, Invoice>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.ClientId, opt => opt.Ignore())
            .ForMember(x => x.CustomerId, opt => opt.Ignore())
            .ForMember(x => x.InvoiceNumber, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyAmount, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyId, opt => opt.Ignore())
            .ForMember(x => x.CustomerCurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyAmount, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyId, opt => opt.Ignore())
            .ForMember(x => x.ClientCurrencyCode, opt => opt.Ignore())
            .ForMember(x => x.ClientBankAccountNumber, opt => opt.Ignore())
            .ForMember(x => x.PaymentStatus, opt => opt.Ignore())
            .ForMember(x => x.ItemIds, opt => opt.Ignore())
            .ForMember(x => x.IsDeleted, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceModel>()
            .ForMember(x => x.ClientName, opt => opt.Ignore())
            .ForMember(x => x.CustomerName, opt => opt.Ignore())
            .ForMember(x => x.Items, opt => opt.Ignore())
            .ForMember(x => x.Amount, opt => opt.MapFrom(scr => scr.CustomerCurrencyAmount))
            .ForMember(x => x.CurrencyCode, opt => opt.MapFrom(scr => scr.CustomerCurrencyCode));
    }
}
