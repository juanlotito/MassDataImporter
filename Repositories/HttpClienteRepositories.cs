using importacionmasiva.api.net.Models;
using importacionmasiva.api.net.Repositories.Interface;

namespace importacionmasiva.api.net.Repositories
{
    public class HttpClienteRepositories : IHttpClienteRepositories
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpClienteRepositories> log;

        public HttpClienteRepositories(ILogger<HttpClienteRepositories> log)
        {
            _client = new HttpClient();
            this.log = log;
        }

        public void SetHeader(Dictionary<string, string> headers)
        {
            foreach (var (key, value) in headers)
            {
                _client.DefaultRequestHeaders.Add(key, value);
            }
        }

        private async Task<Response> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return new Response((int)response.StatusCode, false, content);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new Response((int)response.StatusCode, true, content);
            }
        }

        public async Task<Response> GetAsync(string url)
        {
            try
            {
                using (var response = await _client.GetAsync(url))
                {
                    return await HandleResponse(response);
                }
            }
            catch (HttpRequestException ex)
            {
                log.LogError(ex.Message);
                return new Response(500, true, ex.Message);
            }
        }

        public async Task<Response> PostAsync(string url, object body)
        {
            try
            {
                using (var response = await _client.PostAsJsonAsync(url, body))
                {
                    return await HandleResponse(response);
                }
            }
            catch (HttpRequestException ex)
            {
                return new Response(500, true, ex.Message);
            }
        }

        public async Task<Response> PutAsync(string url, object body)
        {
            try
            {
                using (var response = await _client.PutAsJsonAsync(url, body))
                {
                    return await HandleResponse(response);
                }
            }
            catch (HttpRequestException ex)
            {
                return new Response(500, true, ex.Message);
            }
        }

        public async Task<Response> DeleteAsync(string url)
        {
            try
            {
                using (var response = await _client.DeleteAsync(url))
                {
                    return await HandleResponse(response);
                }
            }
            catch (HttpRequestException ex)
            {
                return new Response(500, true, ex.Message);
            }
        }
    }
}
