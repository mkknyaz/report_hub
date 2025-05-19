using AutoFixture;
using Exadel.ReportHub.Csv;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;

namespace Exadel.ReportHub.Tests.Csv;

[TestFixture]
public class CsvImporterTests : BaseTestFixture
{
    private CsvImporter _csvImporter;

    [SetUp]
    public void SetUp()
    {
        _csvImporter = new CsvImporter();
    }

    [Test]
    public void ImportCsv_ValidRequest_ImportsSuccessfully()
    {
        // Arrange
        var expected = Fixture.CreateMany<ImportInvoiceDTO>(3).ToList();
        var csvStream = GenerateCsvStream(expected);

        // Act
        var imported = _csvImporter.Read<ImportInvoiceDTO>(csvStream);

        // Assert
        Assert.That(imported, Is.Not.Null);
        Assert.That(imported.Count, Is.EqualTo(expected.Count));

        for (int i = 0; i < expected.Count; i++)
        {
            var actual = imported[i];
            var expectedItem = expected[i];

            Assert.Multiple(() =>
            {
                Assert.That(actual.CustomerId, Is.EqualTo(expectedItem.CustomerId));
                Assert.That(actual.InvoiceNumber, Is.EqualTo(expectedItem.InvoiceNumber));
                Assert.That(actual.IssueDate.Date, Is.EqualTo(expectedItem.IssueDate.Date));
                Assert.That(actual.DueDate.Date, Is.EqualTo(expectedItem.DueDate.Date));

                Assert.That(actual.ItemIds, Is.EquivalentTo(expectedItem.ItemIds));
            });
        }
    }

    private static MemoryStream GenerateCsvStream(List<ImportInvoiceDTO> items)
    {
        var writer = new StringWriter();

        writer.WriteLine("CustomerId,InvoiceNumber,ItemIds,IssueDate,DueDate");
        foreach (var item in items)
        {
            var joinedItemIds = string.Join(";", item.ItemIds);
            writer.WriteLine($"{item.CustomerId},{item.InvoiceNumber}," +
                $"{joinedItemIds},{item.IssueDate.Date:yyyy-MM-dd},{item.DueDate.Date:yyyy-MM-dd}");
        }

        writer.Flush();
        var stream = new MemoryStream();
        var writerBytes = System.Text.Encoding.UTF8.GetBytes(writer.ToString());
        stream.Write(writerBytes, 0, writerBytes.Length);
        stream.Position = 0;

        return stream;
    }
}
