using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;

namespace importacionmasiva.api.net.Filters
{
    public class DecodeHeader : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("codiusuario", out var codiUsuario))
            {
                var decodedValue = HttpUtility.UrlDecode(codiUsuario);
                context.HttpContext.Items["decodedCodiUsuario"] = decodedValue;
            }

            await next();
        }
    }
}
