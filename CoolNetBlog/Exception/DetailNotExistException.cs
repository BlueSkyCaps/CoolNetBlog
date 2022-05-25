namespace CoolNetBlog.BlogException
{
    public class DetailNotExistException : BlogException
    {
        public DetailNotExistException(string message)
        {
            Message = message;
        }
        public DetailNotExistException()
        {
        }

        public new string Message { get; set; }

    }
}
