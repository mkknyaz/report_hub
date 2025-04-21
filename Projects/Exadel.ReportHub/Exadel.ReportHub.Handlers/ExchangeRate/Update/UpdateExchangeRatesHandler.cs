using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.RA.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Handlers.ExchangeRate.Update;

public record UpdateExchangeRatesRequest : IRequest<Unit>;

public class UpdateExchangeRatesHandler(
    IExchangeRateProvider exchangeRateProvider,
    IExchangeRateRepository exchangeRepository,
    ILogger<UpdateExchangeRatesHandler> logger) : IRequestHandler<UpdateExchangeRatesRequest, Unit>
{
    public async Task<Unit> Handle(UpdateExchangeRatesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var rates = await exchangeRateProvider.GetDailyRatesAsync(cancellationToken);
            if (!rates.Any())
            {
                logger.LogError(Constants.Error.ExchangeRate.EcbReturnsNothing);
            }
            else
            {
                await exchangeRepository.AddManyAsync(rates, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, Constants.Error.ExchangeRate.RatesUpdateError);
        }

        return Unit.Value;
    }
}
