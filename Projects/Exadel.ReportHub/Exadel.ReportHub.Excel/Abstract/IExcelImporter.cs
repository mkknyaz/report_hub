namespace Exadel.ReportHub.Excel.Abstract;

public interface IExcelImporter
{
    IList<TDto> Read<TDto>(Stream excelStream)
        where TDto : new();
}
