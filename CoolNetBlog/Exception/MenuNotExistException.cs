namespace CoolNetBlog.BlogException
{
    public class MenuNotExistException : BlogException
    {
        public MenuNotExistException(string message)
        {
            Message = message;
        }
        public MenuNotExistException()
        {
        }

        public new string Message { get; set; }

    }
}
