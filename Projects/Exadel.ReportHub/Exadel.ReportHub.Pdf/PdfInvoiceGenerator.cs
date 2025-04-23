using Aspose.Pdf;
using Aspose.Pdf.Text;
using Exadel.ReportHub.Pdf.Abstract;
using Exadel.ReportHub.Pdf.Models;
using Exadel.ReportHub.SDK.DTOs.Item;

namespace Exadel.ReportHub.Pdf;

public class PdfInvoiceGenerator : IPdfInvoiceGenerator
{
    public async Task<Stream> GenerateAsync(InvoiceModel invoice, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        var doc = new Document();
        var page = doc.Pages.Add();
        page.PageInfo.Margin = new MarginInfo(Constants.MarginInfo.Page.Left, Constants.MarginInfo.Page.Bottom, Constants.MarginInfo.Page.Right, Constants.MarginInfo.Page.Top);

        var title = new TextFragment($"{Constants.Text.Label.Invoice}: {invoice.PaymentStatus}")
        {
            TextState = { FontSize = Constants.Text.TextStyle.FontSizeTitle, FontStyle = FontStyles.Bold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        page.Paragraphs.Add(title);

        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.InvoiceNumber}: {invoice.InvoiceNumber}"));
        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.IssueDate}: {invoice.IssueDate}"));
        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.DueDate}: {invoice.DueDate}"));
        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.ClientName}: {invoice.ClientName}"));
        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.CustomerName}: {invoice.CustomerName}"));
        page.Paragraphs.Add(new TextFragment($"{Constants.Text.Label.BankAccountNumber}: {invoice.BankAccountNumber}"));

        page.Paragraphs.Add(Constants.Text.NewLine);

        var table = new Table
        {
            DefaultCellPadding = new MarginInfo(
                Constants.MarginInfo.InvoiceTable.Left,
                Constants.MarginInfo.InvoiceTable.Bottom,
                Constants.MarginInfo.InvoiceTable.Right,
                Constants.MarginInfo.InvoiceTable.Top),
            Border = new BorderInfo(BorderSide.All, Constants.BorderInfo.IvoiceTable.Border),
            DefaultCellBorder = new BorderInfo(BorderSide.All, Constants.BorderInfo.IvoiceTable.CellBorder),
            ColumnAdjustment = ColumnAdjustment.AutoFitToWindow
        };

        table.Rows.Add().Cells.Add(nameof(ItemDTO.Name));
        table.Rows[0].Cells.Add(nameof(ItemDTO.Description));
        table.Rows[0].Cells.Add(nameof(ItemDTO.Price));

        foreach (var item in invoice.Items)
        {
            var row = table.Rows.Add();
            row.Cells.Add(item.Name);
            row.Cells.Add(item.Description);
            row.Cells.Add($"{item.Price} {item.CurrencyCode}");
        }

        page.Paragraphs.Add(table);

        page.Paragraphs.Add(Constants.Text.NewLine);
        var total = new TextFragment($"{Constants.Text.Label.Total}: {invoice.Amount} {invoice.CurrencyCode}")
        {
            TextState = { FontSize = Constants.Text.TextStyle.FontSize, FontStyle = FontStyles.Bold },
            HorizontalAlignment = HorizontalAlignment.Left
        };

        page.Paragraphs.Add(total);

        await doc.SaveAsync(stream, cancellationToken);
        stream.Position = 0;

        return stream;
    }
}
