using OfficeOpenXml;
using System.Data;

namespace importacionmasiva.api.net.Utils.Excel
{
    public class ExcelUtils
    {
        public DataTable ReadExcelToDataTable(IFormFile file)
        {
            using var stream = new MemoryStream();

            file.CopyTo(stream);

            using var package = new ExcelPackage(stream);

            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            DataTable table = new DataTable();

            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                table.Columns.Add(firstRowCell.Text);

            for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                DataRow row = table.Rows.Add();
                foreach (var cell in wsRow)
                    row[cell.Start.Column - 1] = cell.Text;
            }

            return table;
        }
    }
}
