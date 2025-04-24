using Exadel.ReportHub.Ecb.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Handlers.ExchangeRate.Update;

public record UpdateExchangeRatesRequest : IRequest<Unit>;

public class UpdateExchangeRatesHandler(IExchangeRateClient exchangeRateProvider, ILogger<UpdateExchangeRatesHandler> logger)
    : IRequestHandler<UpdateExchangeRatesRequest, Unit>
{
    public async Task<Unit> Handle(UpdateExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await exchangeRateProvider.GetDailyRatesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Daily Rates were not loaded successfully.");
        }

        return Unit.Value;
    }
}
