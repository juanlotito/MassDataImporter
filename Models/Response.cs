namespace importacionmasiva.api.net.Models
{
    public class Response
    {
        public Boolean error { get; set; }
        public int status { get; set; }

        public object? body { get; set; }

        public Response(int status, bool error, object body)
        {
            this.status = status;
            this.error = error;
            this.body = body;

        }

    }
}
