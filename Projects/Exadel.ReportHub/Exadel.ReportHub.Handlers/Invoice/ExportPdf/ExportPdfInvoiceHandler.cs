using System.Net.Mime;
using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Notifications.Invoice.Export;
using Exadel.ReportHub.Pdf.Abstract;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.ExportPdf;

public record ExportPdfInvoiceRequest(Guid Id, Guid ClientId) : IRequest<ErrorOr<ExportResult>>;

public class ExportPdfInvoiceHandler(
    IPdfInvoiceGenerator pdfInvoiceGenerator,
    IInvoiceRepository invoiceRepository,
    IItemRepository itemRepository,
    IClientRepository clientRepository,
    ICustomerRepository customerRepository,
    IUserProvider userProvider,
    IPublisher publisher,
    IMapper mapper) : IRequestHandler<ExportPdfInvoiceRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ExportPdfInvoiceRequest request, CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var isSuccess = false;

        try
        {
            var invoice = await invoiceRepository.GetByIdAsync(request.Id, request.ClientId, cancellationToken);
            if (invoice is null)
            {
                return Error.NotFound();
            }

            var itemsTask = itemRepository.GetByIdsAsync(invoice.ItemIds, cancellationToken);
            var clientTask = clientRepository.GetByIdAsync(invoice.ClientId, cancellationToken);
            var customerTask = customerRepository.GetByIdAsync(invoice.CustomerId, cancellationToken);
            await Task.WhenAll(itemsTask, clientTask, customerTask);

            var invoiceModel = mapper.Map<InvoiceModel>(invoice);
            invoiceModel.ClientName = clientTask.Result.Name;
            invoiceModel.CustomerName = customerTask.Result.Name;
            invoiceModel.Items = mapper.Map<IList<ItemDTO>>(itemsTask.Result);

            var stream = await pdfInvoiceGenerator.GenerateAsync(invoiceModel, cancellationToken);

            var exportDto = new ExportResult
            {
                Stream = stream,
                FileName = $"{invoice.InvoiceNumber}{Export.Abstract.Constants.File.Extension.Pdf}",
                ContentType = MediaTypeNames.Application.Pdf
            };
            isSuccess = true;

            return exportDto;
        }
        finally
        {
            var notification = new InvoiceExportedNotification(userId, request.Id, request.ClientId, DateTime.UtcNow, isSuccess);
            await publisher.Publish(notification, cancellationToken);
        }
    }
}
