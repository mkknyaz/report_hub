using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Pdf;
using Exadel.ReportHub.Pdf.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class PdfRegistrations
{
    public static IServiceCollection AddPdf(this IServiceCollection services)
    {
        services.AddSingleton<IPdfInvoiceGenerator, PdfInvoiceGenerator>();

        return services;
    }
}
