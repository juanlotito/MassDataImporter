using importacionmasiva.api.net.Models.ImportacionLog;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Services.Interface;

namespace importacionmasiva.api.net.Services
{
    public class ImportacionLogService : IImportacionLogsService
    {
        private readonly IImportacionLogsRepository _importacionLogsRepository;

        public ImportacionLogService(IImportacionLogsRepository importacionLogsRepository)
        {
            _importacionLogsRepository = importacionLogsRepository;
        }

        public async Task<int> LogInicio(string tablaDestino, string tipoArchivo, string nombreArchivo, string dbNombre, string accionEliminar, bool creoTabla)
        {
            var importacionLog = new ImportacionLog
            {
                TablaDestino = tablaDestino,
                AccionEliminar = accionEliminar,
                CreoTabla = creoTabla, 
                RegistrosAfectados = 0,
                FechaInicio = DateTime.Now,
                TipoArchivo = tipoArchivo,
                NombreArchivo = nombreArchivo,
                DBNombre = dbNombre,
                Estado = "En curso",
                Error = false
            };

            return await _importacionLogsRepository.InsertImportacionLog(importacionLog, dbNombre);
        }

        public async Task LogFinalizacion(int idImportacionLog, bool error, string estado, string registryName, int registrosAfectados, string detalleError = null)
        {
            var fechaFin = DateTime.Now;
            await _importacionLogsRepository.ActualizarImportacionLog(idImportacionLog, fechaFin, estado, error, detalleError, registryName, registrosAfectados);
        }
    }
}
