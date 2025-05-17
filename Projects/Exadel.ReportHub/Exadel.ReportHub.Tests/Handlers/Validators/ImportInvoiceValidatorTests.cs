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
public class ImportInvoiceValidatorTests : BaseTestFixture
{
    private IValidator<ImportInvoiceDTO> _invoiceValidator;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
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
                .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);

            updateInvoiceValidator.RuleFor(x => x.DueDate)
                .NotEmpty()
                .GreaterThan(x => x.IssueDate)
                .WithMessage(Constants.Validation.Invoice.DueDateBeforeIssueDate);
            updateInvoiceValidator.RuleFor(x => x.DueDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);
        });

        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();

        _invoiceValidator = new ImportInvoiceDtoValidator(_invoiceRepositoryMock.Object,
            _itemRepositoryMock.Object, updateInvoiceValidator);
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
    public async Task ValidateAsync_CustomerIdIsEmpty_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
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
    public async Task ValidateAsync_DueDateIsDefault_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
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
        var invoice = SetupValidInvoice();
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
        var invoice = SetupValidInvoice();
        invoice.ItemIds = new List<Guid>();

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.ItemIds)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Item Ids' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ItemDoesntExists_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();

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

    [Test]
    public async Task ValidateAsync_IssueDateIsInFuture_ErrorReturned()
    {
        // Arrange
        var invoice = SetupValidInvoice();
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
        var invoice = SetupValidInvoice();
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

    private ImportInvoiceDTO SetupValidInvoice()
    {
        var invoice = Fixture.Build<ImportInvoiceDTO>()
            .With(x => x.InvoiceNumber, "INV" + new string('1', 8))
            .With(x => x.IssueDate, DateTime.UtcNow.Date.AddDays(-5))
            .With(x => x.DueDate, DateTime.UtcNow.Date.AddDays(30))
            .Create();

        _invoiceRepositoryMock.Setup(x => x.ExistsAsync(invoice.InvoiceNumber, CancellationToken.None))
            .ReturnsAsync(false);

        _itemRepositoryMock.Setup(x => x.AllExistAsync(invoice.ItemIds, CancellationToken.None))
            .ReturnsAsync(true);

        return invoice;
    }
}
