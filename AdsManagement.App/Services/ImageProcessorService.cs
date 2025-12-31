using AdsManagement.App.Interfaces.Service;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace AdsManagement.App.Services
{
    public class ImageProcessorService : IImageProcessorService
    {
        public async Task<Stream> СompressionImageAsync (Stream stream, CancellationToken token = default)
        {
            if (stream.Length == 0 || stream == null)
                throw new ArgumentNullException(nameof(stream), "Image stream data cannot be null or empty");

            stream.Position = 0;
            using var image = await Image.LoadAsync(stream, token);

            image.Mutate(c => c.Resize(new ResizeOptions
            {
                Size = new Size(800, 0),
                Mode = ResizeMode.Max
            }));

            var encoder = new JpegEncoder { Quality = 70 };
            var streamMemory = new MemoryStream();

            await image.SaveAsJpegAsync(streamMemory, encoder, token);

            streamMemory.Position = 0;
            return streamMemory;
        }
        public async Task<byte[]> СompressionImageAsync(byte[] imageBytes, CancellationToken token = default)
        {
            if (imageBytes.Length == 0 || imageBytes == null)
                throw new ArgumentNullException(nameof(imageBytes), "Image data cannot be null or empty");

            //Создание потока из массива байтов 
            using var stream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync(stream, token);

            image.Mutate(c => c.Resize(new ResizeOptions
            {
                Size = new Size(800, 0),
                Mode = ResizeMode.Max
            }));

            var encoder = new JpegEncoder { Quality = 70 };
            using var streamMemory = new MemoryStream();

            await image.SaveAsJpegAsync(streamMemory, encoder, token);

            return streamMemory.ToArray();
        }
    }
}
