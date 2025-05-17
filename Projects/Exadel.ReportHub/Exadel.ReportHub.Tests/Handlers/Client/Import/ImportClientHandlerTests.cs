using System.Text;
using AutoFixture;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Client.Import;
using Exadel.ReportHub.Handlers.Managers.Client;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Client.Import;

[TestFixture]
public class ImportClientHandlerTests : BaseTestFixture
{
    private Mock<IExcelImporter> _excelImporterMock;
    private Mock<IClientManager> _clientManagerMock;
    private Mock<IValidator<CreateClientDTO>> _validatorMock;
    private ImportClientsHandler _handler;

    [SetUp]
    public void Setup()
    {
        _excelImporterMock = new Mock<IExcelImporter>();
        _clientManagerMock = new Mock<IClientManager>();
        _validatorMock = new Mock<IValidator<CreateClientDTO>>();

        _handler = new ImportClientsHandler(
            _excelImporterMock.Object,
            _clientManagerMock.Object,
            _validatorMock.Object);
    }

    [Test]
    public async Task ImportClients_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var createClientDtos = Fixture.Build<CreateClientDTO>().CreateMany(2).ToList();

        var clientDtos = Fixture.Build<ClientDTO>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Excel content"));

        _excelImporterMock
            .Setup(x => x.Read<CreateClientDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(createClientDtos);

        _clientManagerMock
            .Setup(x => x.CreateClientsAsync(
                It.Is<IEnumerable<CreateClientDTO>>(dtos =>
                    dtos.All(dto =>
                        createClientDtos.Any(expectedDto =>
                            expectedDto.BankAccountNumber == dto.BankAccountNumber &&
                            expectedDto.CountryId == dto.CountryId &&
                            expectedDto.Name == dto.Name))),
                CancellationToken.None))
            .ReturnsAsync(clientDtos);

        _validatorMock
            .Setup(x => x.ValidateAsync(
                createClientDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _validatorMock
            .Setup(x => x.ValidateAsync(
                createClientDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "clients.xlsx")
        };

        // Act
        var request = new ImportClientRequest(importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.ImportedCount, Is.EqualTo(2));
    }
}
