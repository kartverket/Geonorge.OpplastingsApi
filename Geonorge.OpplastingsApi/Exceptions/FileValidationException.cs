namespace Geonorge.OpplastingsApi.Exceptions
{
    public class FileValidationException : Exception
    {
        public FileValidationException()
        {
        }

        public FileValidationException(string message) : base(message)
        {
        }

        public FileValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
