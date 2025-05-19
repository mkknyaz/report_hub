using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class CreatePlanValidatorTests : BaseTestFixture
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<IPlanRepository> _planRepositoryMock;
    private Mock<IItemRepository> _itemRepositoryMock;

    private CreatePlanValidator _validator;

    [SetUp]
    public void Setup()
    {
        var updatePlanValidator = new InlineValidator<UpdatePlanDTO>();

        _clientRepositoryMock = new Mock<IClientRepository>();
        _planRepositoryMock = new Mock<IPlanRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();
        _validator = new CreatePlanValidator(
            updatePlanValidator,
            _itemRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _planRepositoryMock.Object);
    }

    [Test]
    public async Task ValidateAsync_ValidPlan_NoErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task ValidateAsync_EmptyPlanId_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();
        plan.ClientId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_PlanIsNotUnique_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();

        _planRepositoryMock
            .Setup(x => x.ExistsForItemByPeriodAsync(
                plan.ItemId,
                plan.ClientId,
                plan.StartDate,
                plan.EndDate,
                CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Plan.AlreadyExistsForItemAndClient));
    }

    [Test]
    public async Task ValidateAsync_EmptyItemId_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();
        plan.ItemId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.ItemId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Item Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ItemDoesNotExist_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();

        _itemRepositoryMock
            .Setup(x => x.ExistsAsync(plan.ItemId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.ItemId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Item.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_ClientDoesNotExist_ErrorReturned()
    {
        // Arrange
        var plan = SetupValidPlan();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(plan.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(plan);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreatePlanDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
    }

    private CreatePlanDTO SetupValidPlan()
    {
        var plan = Fixture.Build<CreatePlanDTO>()
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(-1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(7))
            .Create();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(plan.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        _itemRepositoryMock
            .Setup(x => x.ExistsAsync(plan.ItemId, CancellationToken.None))
            .ReturnsAsync(true);

        _planRepositoryMock
            .Setup(x => x.ExistsForItemByPeriodAsync(plan.ItemId, plan.ClientId, plan.StartDate, plan.EndDate, CancellationToken.None))
            .ReturnsAsync(false);

        return plan;
    }
}
