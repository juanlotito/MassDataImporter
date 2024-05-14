using Dapper;
using importacionmasiva.api.net.DataBase;
using importacionmasiva.api.net.Models.ImportacionLog;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Utils.Exceptions;
using System.Data.SqlClient;
using System.Data;

namespace importacionmasiva.api.net.Repositories
{
    public class ImportacionLogsRepository : BaseRepository , IImportacionLogsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ImportacionLogsRepository(ILogger<ImportacionLog> logger, IDbContext dbContext, IDbConnectionFactory connectionFactory) : base(logger, dbContext)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertImportacionLog(ImportacionLog importacionLog, string registryName)
        {
            try
            {
                string storedProcedure = "usp_ImportacionLogs_Insert";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    var parameters = new
                    {
                        TablaDestino = importacionLog.TablaDestino,
                        AccionEliminar = importacionLog.AccionEliminar,
                        CreoTabla = importacionLog.CreoTabla,
                        RegistrosAfectados = importacionLog.RegistrosAfectados,
                        FechaInicio = importacionLog.FechaInicio,
                        FechaFin = importacionLog.FechaFin,
                        TipoArchivo = importacionLog.TipoArchivo,
                        NombreArchivo = importacionLog.NombreArchivo,
                        DBNombre = importacionLog.DBNombre,
                        Estado = importacionLog.Estado,
                        Error = importacionLog.Error,
                        DetalleError = importacionLog.DetalleError
                    };

                    return await connection.QueryFirstOrDefaultAsync<int>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

        public async Task ActualizarImportacionLog(int idImportacionLogs, DateTime fechaFin, string estado, bool error, string detalleError, string registryName, int registrosAfectados)
        {
            if (idImportacionLogs <= 0)
                throw new ArgumentException("El ID de ImportacionLogs debe ser mayor que cero.", nameof(idImportacionLogs));

            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("El campo Estado no puede estar vacío.", nameof(estado));

            try
            {
                string storedProcedure = "usp_ImportacionLogs_Update";

                using (var connection = _connectionFactory.CreateConnection(registryName))
                {
                    var parameters = new
                    {
                        IdImportacionLogs = idImportacionLogs,
                        FechaFin = fechaFin,
                        Estado = estado,
                        Error = error,
                        DetalleError = detalleError,
                        RegistrosAfectados = registrosAfectados
                    };

                    await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlex)
            {
                if (sqlex.Number == 220)
                    throw new CustomException(422, "Revisá los códigos ingresados, el valor numérico no debe superar 255", sqlex);

                throw new CustomException(sqlex.State.ToString() == "105" ? 404 : 500, sqlex.Message, sqlex);
            }
            catch (Exception ex)
            {
                throw new CustomException(500, ex.Message, ex);
            }
        }

    }
}
