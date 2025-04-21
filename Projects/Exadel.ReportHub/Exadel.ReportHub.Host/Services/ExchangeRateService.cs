using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.ExchangeRate.Update;
using Exadel.ReportHub.SDK.Abstract;
using MediatR;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
public class ExchangeRateService(ISender sender) : IExchangeRateService
{
    public async Task UpdateExchangeRatesAsync()
    {
        await sender.Send(new UpdateExchangeRatesRequest());
    }
}
