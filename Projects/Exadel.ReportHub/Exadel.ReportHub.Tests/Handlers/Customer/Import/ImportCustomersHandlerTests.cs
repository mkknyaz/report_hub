using System.Text;
using AutoFixture;
using AutoMapper;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Customer.Import;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Customer.Import;

[TestFixture]
public class ImportCustomersHandlerTests : BaseTestFixture
{
    private Mock<IExcelImporter> _excelImporterMock;
    private Mock<ICustomerManager> _customerManagerMock;
    private Mock<IValidator<ImportCustomerDTO>> _validatorMock;

    private ImportCustomersHandler _handler;

    [SetUp]
    public void Setup()
    {
        _excelImporterMock = new Mock<IExcelImporter>();
        _customerManagerMock = new Mock<ICustomerManager>();
        _validatorMock = new Mock<IValidator<ImportCustomerDTO>>();
        _handler = new ImportCustomersHandler(
            _excelImporterMock.Object,
            _customerManagerMock.Object,
            _validatorMock.Object,
            Mapper);
    }

    [Test]
    public async Task ImportCustomers_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var importCustomerDtos = Fixture.Build<ImportCustomerDTO>().CreateMany(2).ToList();
        var clientId = Guid.NewGuid();
        var createCustomerDtos = Mapper.Map<List<CreateCustomerDTO>>(importCustomerDtos);
        foreach (var dto in createCustomerDtos)
        {
            dto.ClientId = clientId;
        }

        var customerDtos = Fixture.Build<CustomerDTO>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Excel content"));

        _excelImporterMock
            .Setup(x => x.Read<ImportCustomerDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(importCustomerDtos);

        _customerManagerMock
            .Setup(x => x.CreateCustomersAsync(
                It.Is<IEnumerable<CreateCustomerDTO>>(dtos =>
                    dtos.All(dto =>
                        createCustomerDtos.Any(expectedDto =>
                            expectedDto.ClientId == dto.ClientId &&
                            expectedDto.CountryId == dto.CountryId &&
                            expectedDto.Name == dto.Name &&
                            expectedDto.Email == dto.Email))),
                CancellationToken.None))
            .ReturnsAsync(customerDtos);

        _validatorMock
            .Setup(x => x.ValidateAsync(
                importCustomerDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _validatorMock
            .Setup(x => x.ValidateAsync(
                importCustomerDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "customers.xlsx")
        };

        // Act
        var request = new ImportCustomersRequest(clientId, importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.ImportedCount, Is.EqualTo(2));
    }
}
