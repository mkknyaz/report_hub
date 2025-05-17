using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class ImportDtoValidatorTests : BaseTestFixture
{
    private ImportDtoValidator _validator;
    private Mock<IFormFile> _mockFormFile;

    [SetUp]
    public void Setup()
    {
        _validator = new ImportDtoValidator();
        _mockFormFile = new Mock<IFormFile>();

        _mockFormFile.Setup(f => f.Length).Returns(1024);
        _mockFormFile.Setup(f => f.FileName).Returns("valid.xlsx");
    }

    [Test]
    public async Task ValidateAsync_NullFile_ErrorReturned()
    {
        // Arrange
        var dto = Fixture.Build<ImportDTO>()
                         .With(x => x.File, (IFormFile)null)
                         .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(dto.File)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'File' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_EmptyFileLength_ErrorReturned()
    {
        // Arrange
        _mockFormFile.Setup(f => f.Length).Returns(0);

        var dto = Fixture.Build<ImportDTO>()
                         .With(x => x.File, _mockFormFile.Object)
                         .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("File.Length"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Import.EmptyFileUpload));
    }

    [Test]
    public async Task ValidateAsync_EmptyFileName_ErrorReturned()
    {
        // Arrange
        _mockFormFile.Setup(f => f.FileName).Returns(string.Empty);

        var dto = Fixture.Build<ImportDTO>()
                         .With(x => x.File, _mockFormFile.Object)
                         .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo("File.FileName"));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'File Name' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ValidInput_NoErrorReturned()
    {
        // Arrange
        var dto = Fixture.Build<ImportDTO>()
                         .With(x => x.File, _mockFormFile.Object)
                         .Create();

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }
}
