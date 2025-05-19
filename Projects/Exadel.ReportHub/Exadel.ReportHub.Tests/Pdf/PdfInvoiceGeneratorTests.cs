using AutoFixture;
using Exadel.ReportHub.Pdf;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.Tests.Abstracts;

namespace Exadel.ReportHub.Tests.Pdf;

[TestFixture]
public class PdfInvoiceGeneratorTests : BaseTestFixture
{
    private PdfInvoiceGenerator _pdfInvoiceGenerator;

    [SetUp]
    public void SetUp()
    {
        _pdfInvoiceGenerator = new PdfInvoiceGenerator();
    }

    [Test]
    public async Task GenerateAsync_ValidInvoice_ReturnsNonEmptyStream()
    {
        // Arrange
        var invoice = Fixture.Build<InvoiceModel>()
            .With(x => x.Items, Fixture.CreateMany<ItemDTO>(3).ToList())
            .Create();

        // Act
        var result = await _pdfInvoiceGenerator.GenerateAsync(invoice, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.GreaterThan(0));
        Assert.That(result.CanRead, Is.True);
        Assert.That(result.Position, Is.EqualTo(0));
    }
}
