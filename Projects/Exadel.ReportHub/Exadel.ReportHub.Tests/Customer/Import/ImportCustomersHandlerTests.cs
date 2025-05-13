using System.Text;
using AutoFixture;
using Exadel.ReportHub.Excel.Abstract;
using Exadel.ReportHub.Handlers.Customer.Import;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Customer.Import;

[TestFixture]
public class ImportCustomersHandlerTests : BaseTestFixture
{
    private Mock<IExcelImporter> _excelImporter;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<ICountryBasedEntityManager> _countryBasedEntityManagerMock;
    private Mock<IValidator<CreateCustomerDTO>> _validatorMock;

    private ImportCustomersHandler _handler;

    [SetUp]
    public void Setup()
    {
        _excelImporter = new Mock<IExcelImporter>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _countryBasedEntityManagerMock = new Mock<ICountryBasedEntityManager>();
        _validatorMock = new Mock<IValidator<CreateCustomerDTO>>();
        _handler = new ImportCustomersHandler(
            _excelImporter.Object,
            _customerRepositoryMock.Object,
            _validatorMock.Object,
            _countryBasedEntityManagerMock.Object);
    }

    [Test]
    public async Task ImportCustomers_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var customerDtos = Fixture.Build<CreateCustomerDTO>().CreateMany(2).ToList();
        var customers = Fixture.Build<Data.Models.Customer>().CreateMany(2).ToList();

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Excel content"));

        _excelImporter
            .Setup(x => x.Read<CreateCustomerDTO>(It.Is<Stream>(str => str.Length == memoryStream.Length)))
            .Returns(customerDtos);

        _countryBasedEntityManagerMock
            .Setup(x => x.GenerateEntitiesAsync<CreateCustomerDTO, Data.Models.Customer>(customerDtos, CancellationToken.None))
            .ReturnsAsync(customers);

        _validatorMock
            .Setup(x => x.ValidateAsync(
                customerDtos[0],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _validatorMock
            .Setup(x => x.ValidateAsync(
                customerDtos[1],
                CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        var importDto = new ImportDTO
        {
            File = new FormFile(memoryStream, 0, memoryStream.Length, "formFile", "customers.xlsx")
        };

        // Act
        var request = new ImportCustomersRequest(importDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.ImportedCount, Is.EqualTo(2));

        _customerRepositoryMock.Verify(
                repo => repo.AddManyAsync(
                    It.Is<IList<Data.Models.Customer>>(
                        inv => inv.Count() == 2 &&
                        inv.Any(x =>
                        x.ClientId == customers[0].ClientId &&
                        x.Name == customers[0].Name &&
                        x.Email == customers[0].Email &&
                        x.CountryId == customers[0].CountryId &&
                        x.Country == customers[0].Country &&
                        x.CurrencyId == customers[0].CurrencyId &&
                        x.CurrencyCode == customers[0].CurrencyCode) &&

                        inv.Any(x =>
                        x.ClientId == customers[1].ClientId &&
                        x.Name == customers[1].Name &&
                        x.Email == customers[1].Email &&
                        x.CountryId == customers[1].CountryId &&
                        x.Country == customers[1].Country &&
                        x.CurrencyId == customers[1].CurrencyId &&
                        x.CurrencyCode == customers[1].CurrencyCode)),
                    CancellationToken.None),
                Times.Once);
    }
}
