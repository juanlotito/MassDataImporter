using System.Data;
using System.Data.SqlClient;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string registryKey);
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection(string registryKey)
        {
            var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software")
                                                       .OpenSubKey("DesarrollosInternos")
                                                       .OpenSubKey(registryKey);

            if (rk == null)
            {
                throw new InvalidOperationException("La clave de registro especificada no existe.");
            }

            string connectionString = $"Server={rk.GetValue("Servidor")};Database={rk.GetValue("Base")};User Id={rk.GetValue("Usuario")};Password={rk.GetValue("Contraseña")}";

            return new SqlConnection(connectionString);
        }
    }
