using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Handlers.Managers.Helpers;

public interface ICountryDataFiller
{
    Task FillCountryDataAsync<TEntity>(IList<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, ICountryBasedDocument;
}
