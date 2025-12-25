namespace AdsManagement.App.Interfaces.Service
{
    public interface IFileData
    {
        public string ContentType { get; }
        public long Length { get; }
        public Stream OpenReadStream();
    }
}
