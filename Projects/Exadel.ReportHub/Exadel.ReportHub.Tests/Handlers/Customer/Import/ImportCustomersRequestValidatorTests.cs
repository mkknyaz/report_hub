using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Customer.Import;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Customer.Import;

[TestFixture]
public class ImportCustomersRequestValidatorTests : BaseTestFixture
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private ImportCustomersRequestValidator _validator;
    private Mock<IFormFile> _mockFormFile;

    [SetUp]
    public void Setup()
    {
        var importDtoValidator = new InlineValidator<ImportDTO>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _validator = new ImportCustomersRequestValidator(_clientRepositoryMock.Object, importDtoValidator);

        _mockFormFile = new Mock<IFormFile>();

        _mockFormFile.Setup(f => f.Length).Returns(1024);
        _mockFormFile.Setup(f => f.FileName).Returns("valid.xlsx");
    }

    [Test]
    public async Task ValidateAsync_InvalidFileExtension_ErrorReturned()
    {
        // Arrange
        _mockFormFile.Setup(f => f.FileName).Returns("invalid.txt");
        var importDto = GetValidImportDto();
        var clientId = Guid.NewGuid();
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new ImportCustomersRequest(clientId, importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("ImportDto.File.FileName"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Import.InvalidFileExtension));
    }

    [Test]
    public async Task ValidateAsync_ClientNotExist_ErrorReturned()
    {
        // Arrange
        var importDto = GetValidImportDto();
        var clientId = Guid.NewGuid();
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new ImportCustomersRequest(clientId, importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(request.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_ValidFile_ReturnsTotalAddedCustomers()
    {
        // Arrange
        var importDto = GetValidImportDto();
        var clientId = Guid.NewGuid();
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new ImportCustomersRequest(clientId, importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(0));
    }

    private ImportDTO GetValidImportDto()
    {
        return Fixture.Build<ImportDTO>()
            .With(x => x.File, _mockFormFile.Object)
            .Create();
    }
}
