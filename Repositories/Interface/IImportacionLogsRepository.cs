using importacionmasiva.api.net.Models.ImportacionLog;

namespace importacionmasiva.api.net.Repositories.Interface
{
    public interface IImportacionLogsRepository
    {
        Task<int> InsertImportacionLog(ImportacionLog importacionLog, string registryName);
        Task ActualizarImportacionLog(int idImportacionLogs, DateTime fechaFin, string estado, bool error, string detalleError, string registryName, int registrosAfectados);
    }
}
