namespace importacionmasiva.api.net.Utils.Exceptions
{
    public class CustomException : Exception
    {
        public int Code { get; }

        public CustomException(int code)
        {
            Code = code;
        }

        public CustomException(int code, string message) : base(message)
        {
            Code = code;
        }

        public CustomException(int code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}
