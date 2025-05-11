using AutoMapper;
using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;

namespace Exadel.ReportHub.Handlers.Managers.Common;

public class CountryBasedEntityManager(
    IMapper mapper,
    ICountryRepository countryRepository) : ICountryBasedEntityManager
{
    public async Task<TEntity> GenerateEntityAsync<TDto, TEntity>(TDto entityDto, CancellationToken cancellationToken)
        where TDto : new()
        where TEntity : IDocument, ICountryBasedDocument
    {
        var entity = (await GenerateEntitiesAsync<TDto, TEntity>([entityDto], cancellationToken)).Single();

        return entity;
    }

    public async Task<IList<TEntity>> GenerateEntitiesAsync<TDto, TEntity>(IEnumerable<TDto> entityDtos, CancellationToken cancellationToken)
        where TDto : new()
        where TEntity : IDocument, ICountryBasedDocument
    {
        var entities = mapper.Map<IList<TEntity>>(entityDtos);

        var countryTask = countryRepository.GetByIdsAsync(entities.Select(x => x.CountryId).Distinct(), cancellationToken);

        await Task.WhenAll(countryTask);
        var countries = countryTask.Result.ToDictionary(x => x.Id);

        foreach (var entity in entities)
        {
            var country = countries[entity.CountryId];

            entity.Country = country.Name;
            entity.CurrencyId = country.CurrencyId;
            entity.CurrencyCode = country.CurrencyCode;
            entity.Id = Guid.NewGuid();
        }

        return entities;
    }
}
