using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.Handlers.Managers.Helpers;

public class CountryDataFiller(ICountryRepository countryRepository) : ICountryDataFiller
{
    public async Task FillCountryDataAsync<TEntity>(IList<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, ICountryBasedDocument
    {
        var countryTasks = countryRepository.GetByIdsAsync(entities.Select(x => x.CountryId).Distinct(), cancellationToken);

        await Task.WhenAll(countryTasks);
        var countries = countryTasks.Result.ToDictionary(x => x.Id);

        foreach (var entity in entities)
        {
            var country = countries[entity.CountryId];

            entity.Country = country.Name;
            entity.CurrencyId = country.CurrencyId;
            entity.CurrencyCode = country.CurrencyCode;
        }
    }
}
