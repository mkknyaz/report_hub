using System.Text;
using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.Import;

public class ImportInvoicesHandlerTests : BaseTestFixture
{
    private Mock<ICsvImporter> _csvProcessorMock;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IInvoiceManager> _invoiceManagerMock;
    private Mock<IValidator<CreateInvoiceDTO>> _invoiceValidatorMock;
    private ImportInvoicesHandler _handler;

    [SetUp]
    public void Setup()
    {
        _csvProcessorMock = new Mock<ICsvImporter>();
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
    public async Task ImportInvoices_ValidRequest_ReturnsImportedCount()
    {
        // Arrange
        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>().CreateMany(2).ToList();
        var invoices = Fixture.Build<Data.Models.Invoice>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        _csvProcessorMock
            .Setup(x => x.Read<CreateInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(invoiceDtos);

        _invoiceManagerMock
            .Setup(x => x.GenerateInvoicesAsync(invoiceDtos, CancellationToken.None))
            .ReturnsAsync(invoices);

        foreach(var invoice in invoiceDtos)
        {
            _invoiceValidatorMock
                .Setup(x => x.ValidateAsync(invoice, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());
        }

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
                        inv[0].ClientId == invoices[0].ClientId &&
                        inv[0].CustomerId == invoices[0].CustomerId &&
                        inv[0].InvoiceNumber == invoices[0].InvoiceNumber &&
                        inv[0].IssueDate == invoices[0].IssueDate &&
                        inv[0].DueDate == invoices[0].DueDate &&
                        inv[0].ClientBankAccountNumber == invoices[0].ClientBankAccountNumber &&
                        inv[0].ClientCurrencyId == invoices[0].ClientCurrencyId &&
                        inv[0].ClientCurrencyCode == invoices[0].ClientCurrencyCode &&
                        inv[0].ClientCurrencyAmount == invoices[0].ClientCurrencyAmount &&
                        inv[0].CustomerCurrencyId == invoices[0].CustomerCurrencyId &&
                        inv[0].CustomerCurrencyCode == invoices[0].CustomerCurrencyCode &&
                        inv[0].CustomerCurrencyAmount == invoices[0].CustomerCurrencyAmount &&
                        inv[0].PaymentStatus == invoices[0].PaymentStatus &&
                        inv[0].ItemIds.SequenceEqual(invoices[0].ItemIds) &&

                        inv[1].ClientId == invoices[1].ClientId &&
                        inv[1].CustomerId == invoices[1].CustomerId &&
                        inv[1].InvoiceNumber == invoices[1].InvoiceNumber &&
                        inv[1].IssueDate == invoices[1].IssueDate &&
                        inv[1].DueDate == invoices[1].DueDate &&
                        inv[1].ClientBankAccountNumber == invoices[1].ClientBankAccountNumber &&
                        inv[1].ClientCurrencyId == invoices[1].ClientCurrencyId &&
                        inv[1].ClientCurrencyCode == invoices[1].ClientCurrencyCode &&
                        inv[1].ClientCurrencyAmount == invoices[1].ClientCurrencyAmount &&
                        inv[1].CustomerCurrencyId == invoices[1].CustomerCurrencyId &&
                        inv[1].CustomerCurrencyCode == invoices[1].CustomerCurrencyCode &&
                        inv[1].CustomerCurrencyAmount == invoices[1].CustomerCurrencyAmount &&
                        inv[1].PaymentStatus == invoices[1].PaymentStatus &&
                        inv[1].ItemIds.SequenceEqual(invoices[1].ItemIds)),
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
            new ("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        foreach(var invoice in invoiceDtos)
        {
            _invoiceValidatorMock
                .Setup(x => x.ValidateAsync(invoice, CancellationToken.None))
                .ReturnsAsync(new ValidationResult(errorsInvoice));
        }

        _csvProcessorMock
            .Setup(x => x.Read<CreateInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
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
            new ("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(invoiceDtos[0], CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(invoiceDtos[1], CancellationToken.None))
            .ReturnsAsync(new ValidationResult(errorsInvoice));

        _csvProcessorMock
            .Setup(x => x.Read<CreateInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
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
