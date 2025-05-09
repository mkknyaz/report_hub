using System.Net.Mail;
using System.Net.Mime;
using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Email.Abstract;
using Exadel.ReportHub.Email.Models;
using Exadel.ReportHub.Pdf;
using Exadel.ReportHub.Pdf.Abstract;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;

namespace Exadel.ReportHub.Handlers.TestEmail;

public record TestEmailRequest(Guid InvoiceId, string Email, string Subject) : IRequest<ErrorOr<TestEmailResult>>;
public class TestEmailHandler(
    IEmailSender emailSender,
    IInvoiceRepository invoiceRepository,
    IUserRepository userRepository,
    IItemRepository itemRepository,
    IClientRepository clientRepository,
    ICustomerRepository customerRepository,
    IMapper mapper,
    IPdfInvoiceGenerator pdfInvoiceGenerator,
    IUserProvider userProvider) : IRequestHandler<TestEmailRequest, ErrorOr<TestEmailResult>>
{
    public async Task<ErrorOr<TestEmailResult>> Handle(TestEmailRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
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
            var attachment = new Attachment(stream, $"{invoice.InvoiceNumber}{Constants.File.Extension.Pdf}", MediaTypeNames.Application.Pdf);

            var user = await userRepository.GetByIdAsync(userProvider.GetUserId(), cancellationToken);
            var reportEmail = new ReportEmailModel
            {
                UserName = user.FullName,
                StartDate = DateTime.Now.Date.ToString("dd.MM.yyyy"),
                EndDate = DateTime.Now.Date.AddDays(2).ToString("dd.MM.yyyy"),
                IsSuccess = true
            };

            await emailSender.SendAsync(request.Email, request.Subject, attachment, "Report.html", reportEmail, cancellationToken);
            return new TestEmailResult { IsSent = true };
        }
        catch
        {
            return new TestEmailResult { IsSent = false };
        }
    }
}
