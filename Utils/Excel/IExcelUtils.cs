using System.Data;

namespace importacionmasiva.api.net.Utils.Excel
{
    public interface IExcelUtils
    {
        DataTable ReadExcelToDataTable(IFormFile file);
    }
}
