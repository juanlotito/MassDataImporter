using System.Data;

namespace importacionmasiva.api.net.Utils.Txt
{
    public class TxtUtils
    {
        public DataTable ReadTxtToDataTable(IFormFile file)
        {
            DataTable dataTable = new DataTable();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var columnNames = reader.ReadLine().Split(';');

                var dataTypes = reader.ReadLine().Split(';');

                for (int i = 0; i < columnNames.Length; i++)
                {
                    Type dataType = InferType(dataTypes[i]);
                    dataTable.Columns.Add(columnNames[i].Trim(), dataType);
                }

                AddRowToDataTable(dataTable, dataTypes);

                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(';');
                    AddRowToDataTable(dataTable, values);
                }
            }
            return dataTable;
        }

        private void AddRowToDataTable(DataTable dataTable, string[] values)
        {
            var rowValues = new object[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                string trimmedValue = values[i].Trim();
                rowValues[i] = string.IsNullOrEmpty(trimmedValue) ? DBNull.Value : ConvertToType(trimmedValue, dataTable.Columns[i].DataType);
            }

            dataTable.Rows.Add(rowValues);
        }

        private object ConvertToType(string value, Type type)
        {
            try
            {
                if (type == typeof(decimal))
                    return decimal.Parse(value);
                else if (type == typeof(DateTime))
                    return DateTime.Parse(value);
                else if (type == typeof(int))
                    return int.Parse(value);
                else
                    return value;
            }
            catch
            {
                return DBNull.Value;
            }
        }

        private Type InferType(string data)
        {
            if (decimal.TryParse(data, out decimal decimalResult))
                return typeof(decimal);
            else if (DateTime.TryParse(data, out DateTime dateResult))
                return typeof(DateTime);
            else if (int.TryParse(data, out int intResult))
                return typeof(int);
            else
                return typeof(string);
        }
    }

}
