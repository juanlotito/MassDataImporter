namespace importacionmasiva.api.net.Services.Interface
{
    public interface IImportacionLogsService
    {
        Task LogFinalizacion(int idImportacionLog, bool error, string estado, string registryName, int registrosAfectados, string detalleError = null);
        Task<int> LogInicio(string tablaDestino, string tipoArchivo, string nombreArchivo, string dbNombre, string accionEliminar, bool creoTabla);
    }
}
