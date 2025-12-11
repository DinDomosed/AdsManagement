namespace AdsManagement.App.Exceptions
{
    public class RoleExistsException : Exception
    {
        public RoleExistsException() : base() { }
        public RoleExistsException(string? message) : base(message) { }
        public RoleExistsException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
