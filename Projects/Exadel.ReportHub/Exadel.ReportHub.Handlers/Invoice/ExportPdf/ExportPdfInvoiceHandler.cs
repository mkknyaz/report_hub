using System.Net.Mime;
using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Pdf.Abstract;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.ExportPdf;

public record ExportPdfInvoiceRequest(Guid InvoiceId) : IRequest<ErrorOr<ExportResult>>;

public class ExportPdfInvoiceHandler(
    IPdfInvoiceGenerator pdfInvoiceGenerator,
    IInvoiceRepository invoiceRepository,
    IItemRepository itemRepostitory,
    IClientRepository clientRepostitory,
    ICustomerRepository customerRepository,
    IMapper mapper) : IRequestHandler<ExportPdfInvoiceRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ExportPdfInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return Error.NotFound();
        }

        var itemsTask = itemRepostitory.GetByIdsAsync(invoice.ItemIds, cancellationToken);
        var clientTask = clientRepostitory.GetByIdAsync(invoice.ClientId, cancellationToken);
        var customerTask = customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);
        await Task.WhenAll(itemsTask, clientTask, customerTask);

        var invoiceModel = new InvoiceModel
        {
            ClientName = clientTask.Result.Name,
            CustomerName = customerTask.Result.Name,
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Amount = invoice.Amount,
            CurrencyCode = invoice.CurrencyCode,
            PaymentStatus = (SDK.Enums.PaymentStatus)invoice.PaymentStatus,
            BankAccountNumber = invoice.BankAccountNumber,
            Items = mapper.Map<IList<ItemDTO>>(itemsTask.Result)
        };
        var stream = await pdfInvoiceGenerator.GenerateAsync(invoiceModel, cancellationToken);

        var exportDto = new ExportResult
        {
            Stream = stream,
            FileName = $"{Constants.File.Name.Invoice}{invoice.InvoiceNumber}{Constants.File.Extension.Pdf}",
            ContentType = MediaTypeNames.Application.Pdf
        };
        return exportDto;
    }
}
