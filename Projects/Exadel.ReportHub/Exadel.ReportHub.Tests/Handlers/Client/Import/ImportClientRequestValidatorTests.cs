using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Client.Import;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Client.Import;

[TestFixture]
public class ImportClientRequestValidatorTests : BaseTestFixture
{
    private Mock<IFormFile> _mockFormFile;
    private ImportClientsRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        var importDtoValidator = new InlineValidator<ImportDTO>();
        _validator = new ImportClientsRequestValidator(importDtoValidator);

        _mockFormFile = new Mock<IFormFile>();
    }

    [Test]
    public async Task ValidateAsync_InvalidFileExtension_ErrorReturned()
    {
        // Arrange
        _mockFormFile.Setup(f => f.FileName).Returns("invalid.txt");

        var importDto = Fixture.Build<ImportDTO>()
            .With(x => x.File, _mockFormFile.Object)
            .Create();

        // Act
        var request = new ImportClientRequest(importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("ImportDTO.File.FileName"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Import.InvalidFileExtension));
    }

    [Test]
    public async Task ValidateAsync_ValidFileExtension_ReturnsTotalAddedCustomers()
    {
        // Arrang
        _mockFormFile.Setup(f => f.FileName).Returns("valid.xlsx");

        var importDto = Fixture.Build<ImportDTO>()
            .With(x => x.File, _mockFormFile.Object)
            .Create();

        // Act
        var request = new ImportClientRequest(importDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Has.Count.EqualTo(0));
    }
}
