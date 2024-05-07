using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace importacionmasiva.api.net.Models.Auth
{
    public class AuthenticationModel
    {
        [FromHeader]
        [JsonProperty(PropertyName = "x-access-token")]
        public string token { get; set; } = "x-access-token";

        [FromHeader]
        public string codiusuario { get; set; }
    }
}
