namespace AdsManagement.App.Interfaces.Service
{
    public interface IImageProcessorService
    {
        public Task<byte[]> СompressionImageAsync(byte[] imageBytes, CancellationToken token = default);
        public Task<Stream> СompressionImageAsync(Stream stream, CancellationToken token = default);
    }
}
