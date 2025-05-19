using Aspose.Cells;
using AutoFixture;
using Exadel.ReportHub.Excel;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;

namespace Exadel.ReportHub.Tests.Excel;

[TestFixture]
public class ExcelImporterTests : BaseTestFixture
{
    private ExcelImporter _excelImporter;

    [SetUp]
    public void SetUp()
    {
        _excelImporter = new ExcelImporter();
    }

    [Test]
    public void ImportExcel_ValidRequest_ImportsSuccessfully()
    {
        // Arrange
        var expected = Fixture.CreateMany<CreateCustomerDTO>(3).ToList();
        var stream = GenerateExcelStream(expected);

        // Act
        var imported = _excelImporter.Read<CreateCustomerDTO>(stream);

        // Assert
        Assert.That(imported, Is.Not.Null);
        Assert.That(imported.Count, Is.EqualTo(expected.Count));

        for (int i = 0; i < expected.Count; i++)
        {
            var expectedItem = expected[i];
            var actualItem = imported[i];

            Assert.Multiple(() =>
            {
                Assert.That(actualItem.Name, Is.EqualTo(expectedItem.Name));
                Assert.That(actualItem.CountryId, Is.EqualTo(expectedItem.CountryId));
                Assert.That(actualItem.Email, Is.EqualTo(expectedItem.Email));
                Assert.That(actualItem.ClientId, Is.EqualTo(expectedItem.ClientId));
            });
        }
    }

    private static MemoryStream GenerateExcelStream(List<CreateCustomerDTO> customers)
    {
        var stream = new MemoryStream();
        using var workbook = new Workbook();
        using var worksheet = workbook.Worksheets[0];
        var cells = worksheet.Cells;

        var properties = typeof(CreateCustomerDTO).GetProperties();

        for (int i = 0; i < properties.Length; i++)
        {
            cells[0, i].PutValue(properties[i].Name);
        }

        for (int rowIndex = 0; rowIndex < customers.Count; rowIndex++)
        {
            var customer = customers[rowIndex];
            for (int col = 0; col < properties.Length; col++)
            {
                var value = properties[col].GetValue(customer);
                cells[rowIndex + 1, col].PutValue(value);
            }
        }

        workbook.Save(stream, SaveFormat.Xlsx);
        stream.Position = 0;
        return stream;
    }
}
