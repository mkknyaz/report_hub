using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class CreateInvoiceValidatorTests : BaseTestFixture
{
    private IValidator<CreateInvoiceDTO> _invoiceValidator;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;

    [SetUp]
    public void Setup()
    {
        var importInvoiceValidator = new InlineValidator<ImportInvoiceDTO>();
        importInvoiceValidator.RuleSet("Default", () =>
        {
            importInvoiceValidator.RuleLevelCascadeMode = CascadeMode.Stop;

            importInvoiceValidator.RuleFor(x => x.InvoiceNumber)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Invoice.InvoiceNumberMaxLength)
                .Matches(@"^INV\d+$")
                .WithMessage(Constants.Validation.Invoice.InvalidInvoiceNumberFormat)
                .MustAsync(async (number, cancellationToken) => !await _invoiceRepositoryMock.Object.ExistsAsync(number, cancellationToken))
                .WithMessage(Constants.Validation.Invoice.DuplicateInvoice);
        });

        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();

        _invoiceValidator = new CreateInvoiceDtoValidator(_clientRepositoryMock.Object,
            _customerRepositoryMock.Object, importInvoiceValidator);
    }

    [Test]
    public async Task ValidateAsync_EverythingIsValid_NoErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task ValidateAsync_ClientIdIsEmpty_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
        invoice.ClientId = Guid.Empty;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_InvoiceNumberIsNullOrEmpty_ErrorReturned(string value)
    {
        // Arrange
        var invoice = SetupValidInvoice();
        invoice.InvoiceNumber = value;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors.Any(x => x.PropertyName == nameof(CreateInvoiceDTO.InvoiceNumber)), Is.True);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Invoice Number' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ClientDoesntExists_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();

        _clientRepositoryMock.Setup(x => x.ExistsAsync(invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_CustomerDoesntExistsOnClient_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();

        _customerRepositoryMock.Setup(x => x.ExistsOnClientAsync(invoice.CustomerId, invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.CustomerId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Customer.DoesNotExistOnClient));
    }

    [Test]
    public async Task ValidateAsync_InvoiceNumberBigLength_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
        invoice.InvoiceNumber = "INV122334323423434324233";

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.InvoiceNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"The length of 'Invoice Number' must be " +
            $"{Constants.Validation.Invoice.InvoiceNumberMaxLength} characters or fewer. You entered {invoice.InvoiceNumber.Length} characters."));
    }

    [Test]
    public async Task ValidateAsync_InvoiceNumberNotStartsWithINV_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
        invoice.InvoiceNumber = "123456789";

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.InvoiceNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.InvalidInvoiceNumberFormat));
    }

    [Test]
    public async Task ValidateAsync_InvoiceNumberExists_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();

        _invoiceRepositoryMock.Setup(x => x.ExistsAsync(invoice.InvoiceNumber, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.InvoiceNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.DuplicateInvoice));
    }

    private CreateInvoiceDTO SetupValidInvoice()
    {
        var invoice = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.InvoiceNumber, "INV" + new string('1', 8))
            .With(x => x.IssueDate, DateTime.UtcNow.Date.AddDays(-5))
            .With(x => x.DueDate, DateTime.UtcNow.Date.AddDays(30))
            .Create();

        _clientRepositoryMock.Setup(x => x.ExistsAsync(invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(true);
        _customerRepositoryMock.Setup(x => x.ExistsOnClientAsync(invoice.CustomerId, invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(true);
        _invoiceRepositoryMock.Setup(x => x.ExistsAsync(invoice.InvoiceNumber, CancellationToken.None))
            .ReturnsAsync(false);

        return invoice;
    }
}
