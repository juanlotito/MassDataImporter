namespace importacionmasiva.api.net.Models.ImportacionLog
{
    public class ImportacionLog
    {
        public int IdImportacionLogs { get; set; }
        public string TablaDestino { get; set; }
        public string AccionEliminar { get; set; }
        public bool CreoTabla { get; set; }
        public int RegistrosAfectados { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TipoArchivo { get; set; }
        public string NombreArchivo { get; set; }
        public string DBNombre { get; set; }
        public string Estado { get; set; }
        public bool Error { get; set; }
        public string? DetalleError { get; set; }
    }
}
