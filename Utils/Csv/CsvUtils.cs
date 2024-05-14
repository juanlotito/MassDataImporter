using CsvHelper;
using importacionmasiva.api.net.Utils.Exceptions;
using System.Data;
using System.Globalization;

namespace importacionmasiva.api.net.Utils.Csv
{
    public class CsvUtils : ICsvUtils
    {
        public DataTable ReadCsvToDataTable(IFormFile file)
        {
            try 
            {
                var dataTable = new DataTable();

                if (file == null) 
                    throw new CustomException(400, "No se ha seleccionado ningún archivo.");

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
            catch (Exception ex)
            {
                throw new CustomException(400, $"Hubo un error en la extracción del CSV: {ex.Message}");
            }
        }

    }
}
