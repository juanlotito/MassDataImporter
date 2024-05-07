using importacionmasiva.api.net.Models;

namespace importacionmasiva.api.net.Repositories.Interface
{
    public interface IHttpClienteRepositories
    {
        public void SetHeader(Dictionary<string, string> headers);

        public Task<Response> GetAsync(string url);

        public Task<Response> PostAsync(string url, object content);

        public Task<Response> PutAsync(string url, object body);

        public Task<Response> DeleteAsync(string url);

    }
}
