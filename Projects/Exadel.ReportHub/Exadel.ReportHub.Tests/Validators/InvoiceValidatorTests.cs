using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Validators;

public class InvoiceValidatorTests : BaseTestFixture
{
    private IValidator<CreateInvoiceDTO> _invoiceValidator;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IItemRepository> _itemRepositoryMock;

    [SetUp]
    public void Setup()
    {
        var updateInvoiceValidator = new InlineValidator<UpdateInvoiceDTO>();
        updateInvoiceValidator.RuleSet("Default", () =>
        {
            updateInvoiceValidator.RuleLevelCascadeMode = CascadeMode.Stop;

            updateInvoiceValidator.RuleFor(x => x.IssueDate)
                .NotEmpty()
                .LessThan(DateTime.UtcNow)
                .WithMessage(Constants.Validation.Invoice.IssueDateInFuture);
            updateInvoiceValidator.RuleFor(x => x.IssueDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage(Constants.Validation.Invoice.TimeComponentNotAllowed);

            updateInvoiceValidator.RuleFor(x => x.DueDate)
                .NotEmpty()
                .GreaterThan(x => x.IssueDate)
                .WithMessage(Constants.Validation.Invoice.DueDateBeforeIssueDate);
            updateInvoiceValidator.RuleFor(x => x.DueDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage(Constants.Validation.Invoice.TimeComponentNotAllowed);
        });

        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();

        _invoiceValidator = new CreateInvoiceDtoValidator(_invoiceRepositoryMock.Object, _clientRepositoryMock.Object,
            _customerRepositoryMock.Object, _itemRepositoryMock.Object, updateInvoiceValidator);
    }

    [Test]
    public async Task ValidateAsync_EverythingIsValid_NoErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();

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
        var invoice = GetValidInvoice();
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
    public async Task ValidateAsync_CustomerIdIsEmpty_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.CustomerId = Guid.Empty;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.CustomerId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Customer Id' must not be empty."));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_InvoiceNumberIsNullOrEmpty_ErrorReturned(string value)
    {
        // Arrange
        var invoice = GetValidInvoice();
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
    public async Task ValidateAsync_DueDateIsDefault_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.DueDate = DateTime.MinValue;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.DueDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Due Date' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_IssueDateIsDefault_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.IssueDate = DateTime.MinValue;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.IssueDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Issue Date' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ItemIdsIsEmpty_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.ItemIds = new List<Guid>();

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.ItemIds)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Item Ids' must not be empty."));
    }

    // Client and Customer tests
    [Test]
    public async Task ValidateAsync_ClientDoesntExists_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        var clientId = Guid.NewGuid();
        invoice.ClientId = clientId;
        _clientRepositoryMock.Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
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
    public async Task ValidateAsync_CustomerDoesntExists_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        var customerId = Guid.NewGuid();
        invoice.CustomerId = customerId;
        _customerRepositoryMock.Setup(x => x.ExistsAsync(customerId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.CustomerId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Customer.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_ItemDoesntExists_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        _itemRepositoryMock.Setup(x => x.AllExistAsync(invoice.ItemIds, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.ItemIds)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Item.DoesNotExist));
    }

    // InvoiceNumber tests
    [Test]
    public async Task ValidateAsync_InvoiceNumberBigLength_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
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
        var invoice = GetValidInvoice();
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
        var invoice = GetValidInvoice();
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

    // IssueDate and DueDate tests
    [Test]
    public async Task ValidateAsync_IssueDateIsInFuture_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.IssueDate = DateTime.UtcNow.Date.AddDays(5);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.IssueDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.IssueDateInFuture));
    }

    [Test]
    public async Task ValidateAsync_DueDateIsLessThenIssueDate_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.IssueDate = DateTime.UtcNow.Date.AddDays(-5);
        invoice.DueDate = DateTime.UtcNow.Date.AddDays(-10);

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.DueDate)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.DueDateBeforeIssueDate));
    }

    private CreateInvoiceDTO GetValidInvoice()
    {
        var clientId = Guid.Parse("ba18cc29-c7ff-48c4-9b7b-456bcef231d0");
        var customerId = Guid.Parse("6d024627-568b-4d57-b477-2274c9d807b9");
        var invoiceNumber = "INV20230051";
        var itemIds = new List<Guid>
        {
            Guid.Parse("76fb1a23-2f77-4c26-bf45-fc655f7432e6"),
            Guid.Parse("5c98227f-e9b7-45dd-bfdb-22dddf384598")
        };

        _clientRepositoryMock.Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);
        _customerRepositoryMock.Setup(x => x.ExistsAsync(customerId, CancellationToken.None))
            .ReturnsAsync(true);
        _invoiceRepositoryMock.Setup(x => x.ExistsAsync(invoiceNumber, CancellationToken.None))
            .ReturnsAsync(false);
        _itemRepositoryMock.Setup(x => x.AllExistAsync(itemIds, CancellationToken.None))
            .ReturnsAsync(true);

        return Fixture.Build<CreateInvoiceDTO>()
                .With(x => x.ClientId, clientId)
                .With(x => x.CustomerId, customerId)
                .With(x => x.InvoiceNumber, invoiceNumber)
                .With(x => x.IssueDate, DateTime.UtcNow.Date.AddDays(-5))
                .With(x => x.DueDate, DateTime.UtcNow.Date.AddDays(30))
                .With(x => x.ItemIds, itemIds)
                .Create();
    }
}
