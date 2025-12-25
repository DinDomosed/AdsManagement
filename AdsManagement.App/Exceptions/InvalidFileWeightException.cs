namespace AdsManagement.App.Exceptions
{
    public class InvalidFileWeightException : Exception
    {
        public long FileSize { get; }
        public long LimitSize { get; }

        public InvalidFileWeightException(long currentFileSize, long limitSize) : base("The current file has an unacceptable weight.")
        {
            FileSize = currentFileSize;
            LimitSize = limitSize;
        }
    }
}
