using CsvHelper;
using System.Data;
using System.Globalization;

namespace importacionmasiva.api.net.Utils.Csv
{
    public class CsvUtils : ICsvUtils
    {
        public DataTable ReadCsvToDataTable(IFormFile file)
        {
            var dataTable = new DataTable();
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                using (var dr = new CsvDataReader(csv))
                {
                    dataTable.Load(dr);
                }
            }
            return dataTable;
        }
    }
}
