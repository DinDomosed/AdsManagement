namespace AdsManagement.App.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() : base() { }
        public RoleNotFoundException(string? message) : base(message) { }
        public RoleNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
