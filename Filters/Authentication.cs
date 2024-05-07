using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using importacionmasiva.api.net.Services.Interface;
using importacionmasiva.api.net.Utils;
using importacionmasiva.api.net.Models.Auth;
using System.Text.Json;
using importacionmasiva.api.net.Models;

namespace importacionmasiva.api.net.Filters
{
    public class Autenticacion : Attribute, IAsyncActionFilter
    {

        private readonly IAutenticacionService _IAutenticacionService;

        public Autenticacion(IAutenticacionService _IAutenticacionService)
        {
            this._IAutenticacionService = _IAutenticacionService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string authUrl = Utilitys.GetConfigSection("Auth:url").Value;

            var codigoAplicacion = Utilitys.GetConfigSection("Auth:CodigoAplicacion").Value;

            authUrl += codigoAplicacion;

            AuthenticationModel header = new AuthenticationModel
            {
                token = context.ActionArguments["token"].ToString(),
                codiusuario = context.ActionArguments["codiusuario"].ToString()
            };

            var resp = await _IAutenticacionService.Auth(authUrl, header);

            if (resp.status != 200)
            {
                var dataError = JsonSerializer.Deserialize<Response>(resp.body.ToString());
                context.Result = new UnauthorizedObjectResult(new Response(dataError.status, true, dataError.body.ToString()));
            }
            else { await next(); }
        }
    }
}
