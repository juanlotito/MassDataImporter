using importacionmasiva.api.net.Models;
using importacionmasiva.api.net.Models.Auth;

namespace importacionmasiva.api.net.Services.Interface
{
    public interface IAutenticacionService
    {
        public Task<Response> Auth(string authUrl, AuthenticationModel header);
    }
}
