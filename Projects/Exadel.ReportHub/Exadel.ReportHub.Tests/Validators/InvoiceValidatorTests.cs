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
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;

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
                .WithMessage(Constants.Validation.Invoice.IssueDateErrorMessage);
            updateInvoiceValidator.RuleFor(x => x.IssueDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage(Constants.Validation.Invoice.TimeComponentErrorMassage);

            updateInvoiceValidator.RuleFor(x => x.DueDate)
                .NotEmpty()
                .GreaterThan(x => x.IssueDate)
                .WithMessage(Constants.Validation.Invoice.DueDateErrorMessage);
            updateInvoiceValidator.RuleFor(x => x.DueDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage(Constants.Validation.Invoice.TimeComponentErrorMassage);

            updateInvoiceValidator.RuleFor(x => x.PaymentStatus)
                .IsInEnum();

            updateInvoiceValidator.RuleFor(x => x.BankAccountNumber)
                .NotEmpty()
                .Length(Constants.Validation.Invoice.BankAccountNumberMinLength, Constants.Validation.Invoice.BankAccountNumberMaxLength)
                .Matches(@"^[A-Z]{2}\d+$")
                .WithMessage(Constants.Validation.Invoice.BankAccountNumberErrorMessage);
        });

        _clientRepositoryMock = new Mock<IClientRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _invoiceValidator = new CreateInvoiceDtoValidator(_customerRepositoryMock.Object, _clientRepositoryMock.Object, _invoiceRepositoryMock.Object, updateInvoiceValidator);
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

    // Not Empty Tests
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
    public async Task ValidateAsync_AmountIsZero_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.Amount = 0m;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.Amount)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Amount' must be greater than '0'."));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_CurrencyIsNullOrEmpty_ErrorReturned(string value)
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.Currency = value;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors.Any(x => x.PropertyName == nameof(CreateInvoiceDTO.Currency)), Is.True);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Currency' must not be empty."));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_BankAccountNumberIsNullOrEmpty_ErrorReturned(string value)
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.BankAccountNumber = value;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
        Assert.That(result.Errors.Any(x => x.PropertyName == nameof(CreateInvoiceDTO.BankAccountNumber)), Is.True);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Bank Account Number' must not be empty."));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.ClientDoesntExistsErrorMessage));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.CustomerDoesntExistsErrorMessage));
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
            $"{Constants.Validation.Invoice.InvoiceMaximumNumberLength} characters or fewer. You entered {invoice.InvoiceNumber.Length} characters."));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.InvoiceNumberErrorMessage));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.InvoiceNumberExistsMessage));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.IssueDateErrorMessage));
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
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.DueDateErrorMessage));
    }

    // Amount tests
    [Test]
    public async Task ValidateAsync_AmountIsNegative_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.Amount = -1000.00m;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.Amount)));
    }

    // BankAccountNumber tests
    [Test]
    public async Task ValidateAsync_BankAccountNumberIsNotNumericAndNotDash_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.BankAccountNumber = "234432423432432432";

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Invoice.BankAccountNumberErrorMessage));
    }

    [Test]
    [TestCase("2232323-322323-322323-234234233424-23423423")]
    [TestCase("2332-23")]
    public async Task ValidateAsync_BankAccountNumberNotCorrectLength_ErrorReturned(string bankAccountNumber)
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.BankAccountNumber = bankAccountNumber;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'Bank Account Number' must be between {Constants.Validation.Invoice.BankAccountNumberMinLength} and " +
            $"{Constants.Validation.Invoice.BankAccountNumberMaxLength} characters. You entered {bankAccountNumber.Length} characters."));
    }

    // Currency tests
    [Test]
    public async Task ValidateAsync_CurrencyCodeNotFixedLength_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.Currency = "US";

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.Currency)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"Currency must be exactly {Constants.Validation.Invoice.CurrencyCodeLength} characters long."));
    }

    [Test]
    public async Task ValidateAsync_CurrencyCodeNotUpperCase_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.Currency = "usd";

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.Currency)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"Currency code must be exactly {Constants.Validation.Invoice.CurrencyCodeLength} uppercase letters."));
    }

    // PaymentStatus tests
    [Test]
    public async Task ValidateAsync_PaymentStatusIsInvalid_ErrorReturned()
    {
        // Arrange
        var invoice = GetValidInvoice();
        invoice.PaymentStatus = (PaymentStatus)100;

        // Act
        var result = await _invoiceValidator.TestValidateAsync(invoice);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateInvoiceDTO.PaymentStatus)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'Payment Status' has a range of values which does not include '{(int)invoice.PaymentStatus}'."));
    }

    private CreateInvoiceDTO GetValidInvoice()
    {
        var clientId = Guid.Parse("BA18CC29-C7FF-48C4-9B7B-456BCEF231D0");
        var customerId = Guid.Parse("6D024627-568B-4D57-B477-2274C9D807B9");
        var invoiceNumber = "INV20230051";

        _clientRepositoryMock.Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);
        _customerRepositoryMock.Setup(x => x.ExistsAsync(customerId, CancellationToken.None))
            .ReturnsAsync(true);
        _invoiceRepositoryMock.Setup(x => x.ExistsAsync(invoiceNumber, CancellationToken.None))
            .ReturnsAsync(false);

        return Fixture.Build<CreateInvoiceDTO>()
                .With(x => x.ClientId, clientId )
                .With(x => x.CustomerId, customerId)
                .With(x => x.InvoiceNumber, invoiceNumber)
                .With(x => x.IssueDate, DateTime.UtcNow.Date.AddDays(-5))
                .With(x => x.DueDate, DateTime.UtcNow.Date.AddDays(30))
                .With(x => x.Currency, "USD")
                .With(x => x.BankAccountNumber, "GE359459402653871205990733")
                .Create();
    }
}
