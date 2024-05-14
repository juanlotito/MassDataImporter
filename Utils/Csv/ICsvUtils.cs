using System.Data;

namespace importacionmasiva.api.net.Utils.Csv
{
    public interface ICsvUtils
    {
        DataTable ReadCsvToDataTable(IFormFile file);
    }
}
