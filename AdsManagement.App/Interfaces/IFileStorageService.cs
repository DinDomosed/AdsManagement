namespace AdsManagement.App.Interfaces
{
    public interface IFileStorageService
    {
        public Task SaveAsync(byte[] bytes, string fullPath, CancellationToken token = default);
        public Task SaveAsync(Stream stream, string fullPath, CancellationToken token = default);
        public bool FileExists(string fullPath);
        public Task DeleteAsync(string fullPath, CancellationToken token = default);
    }
}
