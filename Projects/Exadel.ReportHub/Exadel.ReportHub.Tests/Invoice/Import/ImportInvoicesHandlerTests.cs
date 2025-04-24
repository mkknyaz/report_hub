using System.Text;
using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.Handlers.Managers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.Import;

public class ImportInvoicesHandlerTests : BaseTestFixture
{
    private Mock<ICsvProcessor> _csvProcessorMock;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IInvoiceManager> _invoiceManagerMock;
    private Mock<IValidator<CreateInvoiceDTO>> _invoiceValidatorMock;

    private ImportInvoicesHandler _handler;

    [SetUp]
    public void Setup()
    {
        _csvProcessorMock = new Mock<ICsvProcessor>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _invoiceManagerMock = new Mock<IInvoiceManager>();
        _invoiceValidatorMock = new Mock<IValidator<CreateInvoiceDTO>>();
        _handler = new ImportInvoicesHandler(
            _csvProcessorMock.Object,
            _invoiceRepositoryMock.Object,
            _invoiceManagerMock.Object,
            _invoiceValidatorMock.Object);
    }

    [Test]
    public async Task ImportInvoices_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>().CreateMany(2).ToList();
        var invoices = Fixture.Build<Data.Models.Invoice>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        _csvProcessorMock
            .Setup(x => x.ReadInvoices(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(invoiceDtos);

        _invoiceManagerMock
            .Setup(x => x.GenerateInvoicesAsync(invoiceDtos, CancellationToken.None))
            .ReturnsAsync(invoices);

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.ImportedCount, Is.EqualTo(2));

        _invoiceRepositoryMock.Verify(
                repo => repo.AddManyAsync(
                    It.Is<IList<Data.Models.Invoice>>(
                        inv => inv.Count() == 2 &&
                        inv.Any(x =>
                        x.ClientId == invoices[0].ClientId &&
                        x.CustomerId == invoices[0].CustomerId &&
                        x.InvoiceNumber == invoices[0].InvoiceNumber &&
                        x.IssueDate == invoices[0].IssueDate &&
                        x.DueDate == invoices[0].DueDate &&
                        x.Amount == invoices[0].Amount &&
                        x.CurrencyId == invoices[0].CurrencyId &&
                        x.CurrencyCode == invoices[0].CurrencyCode &&
                        (int)x.PaymentStatus == (int)invoices[0].PaymentStatus &&
                        x.BankAccountNumber == invoices[0].BankAccountNumber) &&

                        inv.Any(x =>
                        x.ClientId == invoices[1].ClientId &&
                        x.CustomerId == invoices[1].CustomerId &&
                        x.InvoiceNumber == invoices[1].InvoiceNumber &&
                        x.IssueDate == invoices[1].IssueDate &&
                        x.DueDate == invoices[1].DueDate &&
                        x.Amount == invoices[1].Amount &&
                        x.CurrencyId == invoices[1].CurrencyId &&
                        x.CurrencyCode == invoices[1].CurrencyCode &&
                        (int)x.PaymentStatus == (int)invoices[1].PaymentStatus &&
                        x.BankAccountNumber == invoices[1].BankAccountNumber)),
                    CancellationToken.None),
                Times.Once);
    }

    [Test]
    public async Task ImportInvoices_WhenAllInvoicesInvalid_ReturnsValidationErrors()
    {
        // Arrange
        var expectedErrors = new List<string>()
        {
            "Row 1: Bank account number must only contain digits and dashes.",
            "Row 1: Issue date cannot be in the future",
            "Row 2: Bank account number must only contain digits and dashes.",
            "Row 2: Issue date cannot be in the future"
        };

        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        var errorsInvoice = new List<ValidationFailure>
        {
            new ValidationFailure("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ValidationFailure("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult(errorsInvoice));

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult(errorsInvoice));

        _csvProcessorMock
            .Setup(x => x.ReadInvoices(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(invoiceDtos);

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(expectedErrors.Count));
        Assert.That(result.Errors.All(e => e.Type == ErrorType.Validation), Is.True);
        Assert.That(result.Errors[0].Description, Is.EqualTo(expectedErrors[0]));
        Assert.That(result.Errors[1].Description, Is.EqualTo(expectedErrors[1]));
        Assert.That(result.Errors[2].Description, Is.EqualTo(expectedErrors[2]));
        Assert.That(result.Errors[3].Description, Is.EqualTo(expectedErrors[3]));

        _invoiceRepositoryMock.Verify(
                repo => repo.AddManyAsync(
                    It.IsAny<IList<Data.Models.Invoice>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Test]
    public async Task ImportInvoices_WhenTheOnlyInvoiceInvalid_ReturnsValidationErrors()
    {
        // Arrange
        var expectedErrors = new List<string>()
        {
            "Row 2: Bank account number must only contain digits and dashes.",
            "Row 2: Issue date cannot be in the future"
        };

        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        var errorsInvoice = new List<ValidationFailure>
        {
            new ValidationFailure("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ValidationFailure("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                invoiceDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult(errorsInvoice));

        _csvProcessorMock
            .Setup(x => x.ReadInvoices(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(invoiceDtos);

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(expectedErrors.Count));
        Assert.That(result.Errors.All(e => e.Type == ErrorType.Validation), Is.True);
        Assert.That(result.Errors[0].Description, Is.EqualTo(expectedErrors[0]));
        Assert.That(result.Errors[1].Description, Is.EqualTo(expectedErrors[1]));

        _invoiceRepositoryMock.Verify(
                repo => repo.AddManyAsync(
                    It.IsAny<IList<Data.Models.Invoice>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
    }
}
