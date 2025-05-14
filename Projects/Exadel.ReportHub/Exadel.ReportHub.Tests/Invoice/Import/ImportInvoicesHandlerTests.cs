using System.Text;
using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.Handlers.Managers.Invoice;
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
    private Mock<ICsvImporter> _csvImporterMock;
    private Mock<IInvoiceManager> _invoiceManagerMock;
    private Mock<IValidator<CreateInvoiceDTO>> _invoiceValidatorMock;
    private ImportInvoicesHandler _handler;

    [SetUp]
    public void Setup()
    {
        _csvImporterMock = new Mock<ICsvImporter>();
        _invoiceManagerMock = new Mock<IInvoiceManager>();
        _invoiceValidatorMock = new Mock<IValidator<CreateInvoiceDTO>>();
        _handler = new ImportInvoicesHandler(
            _csvImporterMock.Object,
            _invoiceManagerMock.Object,
            _invoiceValidatorMock.Object,
            Mapper);
    }

    [Test]
    public async Task ImportInvoices_ValidRequest_ReturnsImportedCount()
    {
        // Arrange
        var importInvoiceDtos = Fixture.Build<ImportInvoiceDTO>().CreateMany(2).ToList();
        var clientId = Guid.NewGuid();
        var createInvoiceDtos = Mapper.Map<List<CreateInvoiceDTO>>(importInvoiceDtos);
        foreach (var dto in createInvoiceDtos)
        {
            dto.ClientId = clientId;
        }

        var invoiceDtos = Fixture.Build<InvoiceDTO>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        _csvImporterMock
            .Setup(x => x.Read<ImportInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(importInvoiceDtos);

        _invoiceManagerMock
            .Setup(x => x.CreateInvoicesAsync(
                It.Is<IEnumerable<CreateInvoiceDTO>>(dtos =>
                    dtos.All(dto =>
                        createInvoiceDtos.Any(expectedDto =>
                            expectedDto.ClientId == dto.ClientId &&
                            expectedDto.CustomerId == dto.CustomerId &&
                            expectedDto.InvoiceNumber == dto.InvoiceNumber &&
                            expectedDto.ItemIds.SequenceEqual(dto.ItemIds) &&
                            expectedDto.IssueDate == dto.IssueDate &&
                            expectedDto.DueDate == dto.DueDate))),
                CancellationToken.None))
            .ReturnsAsync(invoiceDtos);

        foreach (var expectedDto in createInvoiceDtos)
        {
            _invoiceValidatorMock
                .Setup(x => x.ValidateAsync(
                    It.Is<CreateInvoiceDTO>(dto =>
                        expectedDto.ClientId == dto.ClientId &&
                        expectedDto.CustomerId == dto.CustomerId &&
                        expectedDto.InvoiceNumber == dto.InvoiceNumber &&
                        expectedDto.ItemIds.SequenceEqual(dto.ItemIds) &&
                        expectedDto.IssueDate == dto.IssueDate &&
                        expectedDto.DueDate == dto.DueDate),
                    CancellationToken.None))
                .ReturnsAsync(new ValidationResult());
        }

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(clientId, importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.ImportedCount, Is.EqualTo(2));
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

        var importInvoiceDtos = Fixture.Build<ImportInvoiceDTO>().CreateMany(2).ToList();
        var clientId = Guid.NewGuid();
        var createInvoiceDtos = Mapper.Map<List<CreateInvoiceDTO>>(importInvoiceDtos);
        foreach (var dto in createInvoiceDtos)
        {
            dto.ClientId = clientId;
        }

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        var errorsInvoice = new List<ValidationFailure>
        {
            new ("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        foreach (var expectedDto in createInvoiceDtos)
        {
            _invoiceValidatorMock
                .Setup(x => x.ValidateAsync(
                    It.Is<CreateInvoiceDTO>(dto =>
                        expectedDto.ClientId == dto.ClientId &&
                        expectedDto.CustomerId == dto.CustomerId &&
                        expectedDto.InvoiceNumber == dto.InvoiceNumber &&
                        expectedDto.ItemIds.SequenceEqual(dto.ItemIds) &&
                        expectedDto.IssueDate == dto.IssueDate &&
                        expectedDto.DueDate == dto.DueDate),
                    CancellationToken.None))
                .ReturnsAsync(new ValidationResult(errorsInvoice));
        }

        _csvImporterMock
            .Setup(x => x.Read<ImportInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(importInvoiceDtos);

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(clientId, importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(expectedErrors.Count));
        Assert.That(result.Errors.All(e => e.Type == ErrorType.Validation), Is.True);
        Assert.That(result.Errors[0].Description, Is.EqualTo(expectedErrors[0]));
        Assert.That(result.Errors[1].Description, Is.EqualTo(expectedErrors[1]));
        Assert.That(result.Errors[2].Description, Is.EqualTo(expectedErrors[2]));
        Assert.That(result.Errors[3].Description, Is.EqualTo(expectedErrors[3]));
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

        var importInvoiceDtos = Fixture.Build<ImportInvoiceDTO>().CreateMany(2).ToList();
        var clientId = Guid.NewGuid();
        var createInvoiceDtos = Mapper.Map<List<CreateInvoiceDTO>>(importInvoiceDtos);
        foreach (var dto in createInvoiceDtos)
        {
            dto.ClientId = clientId;
        }

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("CSV content"));

        var errorsInvoice = new List<ValidationFailure>
        {
            new ("BankAccountNumber", "Bank account number must only contain digits and dashes."),
            new ("IssueDateErrorMessage", "Issue date cannot be in the future")
        };

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                It.Is<CreateInvoiceDTO>(dto =>
                    createInvoiceDtos[0].ClientId == dto.ClientId &&
                    createInvoiceDtos[0].CustomerId == dto.CustomerId &&
                    createInvoiceDtos[0].InvoiceNumber == dto.InvoiceNumber &&
                    createInvoiceDtos[0].ItemIds.SequenceEqual(dto.ItemIds) &&
                    createInvoiceDtos[0].IssueDate == dto.IssueDate &&
                    createInvoiceDtos[0].DueDate == dto.DueDate),
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _invoiceValidatorMock
            .Setup(x => x.ValidateAsync(
                It.Is<CreateInvoiceDTO>(dto =>
                    createInvoiceDtos[1].ClientId == dto.ClientId &&
                    createInvoiceDtos[1].CustomerId == dto.CustomerId &&
                    createInvoiceDtos[1].InvoiceNumber == dto.InvoiceNumber &&
                    createInvoiceDtos[1].ItemIds.SequenceEqual(dto.ItemIds) &&
                    createInvoiceDtos[1].IssueDate == dto.IssueDate &&
                    createInvoiceDtos[1].DueDate == dto.DueDate),
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult(errorsInvoice));

        _csvImporterMock
            .Setup(x => x.Read<ImportInvoiceDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(importInvoiceDtos);

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "invoices.csv")
        };

        // Act
        var request = new ImportInvoicesRequest(clientId, importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(expectedErrors.Count));
        Assert.That(result.Errors.All(e => e.Type == ErrorType.Validation), Is.True);
        Assert.That(result.Errors[0].Description, Is.EqualTo(expectedErrors[0]));
        Assert.That(result.Errors[1].Description, Is.EqualTo(expectedErrors[1]));
    }
}
