using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Handlers.Managers.Common;

public interface ICountryBasedEntityManager
{
    Task<TEntity> GenerateEntityAsync<TDto, TEntity>(TDto entityDto, CancellationToken cancellationToken)
        where TDto : new()
        where TEntity : IDocument, ICountryBasedDocument;

    Task<IList<TEntity>> GenerateEntitiesAsync<TDto, TEntity>(IEnumerable<TDto> entityDtos, CancellationToken cancellationToken)
        where TDto : new()
        where TEntity : IDocument, ICountryBasedDocument;
}
