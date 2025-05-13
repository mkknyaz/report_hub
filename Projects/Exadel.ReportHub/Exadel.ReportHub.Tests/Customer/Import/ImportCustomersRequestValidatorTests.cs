using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Customer.Import;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Customer.Import;

[TestFixture]
public class ImportCustomersRequestValidatorTests : BaseTestFixture
{
    private ImportCustomersRequestValidator _validator;
    private Mock<IFormFile> _mockFormFile;

    [SetUp]
    public void Setup()
    {
        var importDtoValidator = new InlineValidator<ImportDTO>();
        _validator = new ImportCustomersRequestValidator(importDtoValidator);

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

        // Act
        var request = new ImportCustomersRequest(importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("ImportDTO.File.FileName"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Import.InvalidFileExtension));
    }

    [Test]
    public async Task ValidateAsync_ValidFile_ReturnsTotalAddedCustomers()
    {
        // Arrange
        var importDto = GetValidImportDto();

        // Act
        var request = new ImportCustomersRequest(importDto);
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
