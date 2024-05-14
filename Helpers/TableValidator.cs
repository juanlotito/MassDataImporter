using importacionmasiva.api.net.Models.DB;
using importacionmasiva.api.net.Utils.Exceptions;
using System.Data;

namespace importacionmasiva.api.net.Helpers
{
    public class TableValidator
    {
        public void ValidateTableDefinition(TableDefinition tableDefinition, DataTable dataTable)
        {
            if (tableDefinition.Columns.Count != dataTable.Columns.Count)
                throw new CustomException(400, "El número de columnas no coincide con el de la tabla existente.");

            for (int i = 0; i < tableDefinition.Columns.Count; i++)
            {
                if (tableDefinition.Columns[i].Name != dataTable.Columns[i].ColumnName)
                    throw new CustomException(400, $"El nombre de la columna '{dataTable.Columns[i].ColumnName}' no coincide con el de la tabla: '{tableDefinition.Columns[i].Name}'.");
            }
        }
    }

}
