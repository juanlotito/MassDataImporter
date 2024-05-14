using System.Data;

namespace importacionmasiva.api.net.Utils.Txt
{
    public interface ITxtUtils
    {
        public DataTable ReadTxtToDataTable(IFormFile file);
    }
}
