using AutoMapper;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Handlers.Managers.Invoice;

public class InvoiceManager(
    IClientRepository clientRepository,
    ICustomerRepository customerRepository,
    IItemRepository itemRepository,
    ICurrencyConverter currencyConverter,
    IMapper mapper) : IInvoiceManager
{
    public async Task<Data.Models.Invoice> GenerateInvoiceAsync(CreateInvoiceDTO invoiceDto, CancellationToken cancellationToken)
    {
        var invoice = (await GenerateInvoicesAsync([invoiceDto], cancellationToken)).Single();

        return invoice;
    }

    public async Task<IList<Data.Models.Invoice>> GenerateInvoicesAsync(IEnumerable<CreateInvoiceDTO> invoiceDtos, CancellationToken cancellationToken)
    {
        var invoices = mapper.Map<IList<Data.Models.Invoice>>(invoiceDtos);

        var customersTask = customerRepository.GetByIdsAsync(invoices.Select(x => x.CustomerId).Distinct(), cancellationToken);
        var itemsTask = itemRepository.GetByIdsAsync(invoices.SelectMany(x => x.ItemIds).Distinct(), cancellationToken);
        var clientTask = clientRepository.GetByIdsAsync(invoices.Select(x => x.ClientId).Distinct(), cancellationToken);

        await Task.WhenAll(customersTask, itemsTask, clientTask);

        var customers = customersTask.Result.ToDictionary(x => x.Id);
        var items = itemsTask.Result.ToDictionary(x => x.Id);
        var clients = clientTask.Result.ToDictionary(x => x.Id);

        foreach (var invoice in invoices)
        {
            var currencyCode = customers[invoice.CustomerId].CurrencyCode;
            var conversionTasks = invoice.ItemIds.Select(id => items[id]).GroupBy(x => x.CurrencyCode)
                .Select(group => currencyConverter.ConvertAsync(group.Sum(x => x.Price), group.Key, currencyCode, invoice.IssueDate, cancellationToken));

            invoice.Id = Guid.NewGuid();
            invoice.CustomerCurrencyAmount = (await Task.WhenAll(conversionTasks)).Sum();
            invoice.CustomerCurrencyId = customers[invoice.CustomerId].CurrencyId;
            invoice.CustomerCurrencyCode = currencyCode;
            invoice.ClientBankAccountNumber = clients[invoice.ClientId].BankAccountNumber;

            var clientCurrencyCode = clients[invoice.ClientId].CurrencyCode;

            invoice.ClientCurrencyAmount = await currencyConverter.ConvertAsync(
                invoice.CustomerCurrencyAmount,
                invoice.CustomerCurrencyCode,
                clients[invoice.ClientId].CurrencyCode,
                invoice.IssueDate,
                cancellationToken);

            invoice.ClientCurrencyCode = clientCurrencyCode;
            invoice.ClientCurrencyId = clients[invoice.ClientId].CurrencyId;
        }

        return invoices;
    }
}
