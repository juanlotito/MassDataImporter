using importacionmasiva.api.net.Models.Auth;
using importacionmasiva.api.net.Repositories.Interface;
using importacionmasiva.api.net.Services.Interface;

namespace importacionmasiva.api.net.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IHttpClienteRepository _IHttpClienteRepositories;

        public AutenticacionService(IHttpClienteRepository _IHttpClienteRepositories)
        {
            this._IHttpClienteRepositories = _IHttpClienteRepositories;
        }

        public async Task<Models.Response> Auth(string authUrl, AuthenticationModel header)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { "x-access-token", header.token },
                    { "codiusuario", header.codiusuario }
                };
                _IHttpClienteRepositories.SetHeader(headers);

                Models.Response resp = await _IHttpClienteRepositories.PostAsync(authUrl, new { });
                return resp;
            }
            catch
            {
                throw;
            }
        }
    }
}
