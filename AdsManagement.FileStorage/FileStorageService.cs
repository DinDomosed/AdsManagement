using AdsManagement.App.Interfaces;


namespace AdsManagement.FileStorage
{
    public class FileStorageService : IFileStorageService
    {
        public async Task SaveAsync(byte[] bytes, string fullPath, CancellationToken token = default)
        {
            if (bytes.Length == 0 || bytes == null)
                throw new ArgumentNullException(nameof(bytes), "File data cannot be null or empty");

            var pathToDir = Path.GetDirectoryName(fullPath);
            var dirInfo = new DirectoryInfo(pathToDir);
            if (!dirInfo.Exists)
                dirInfo.Create();

            using FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.WriteAsync(bytes, 0, bytes.Length, token);
        }
        public async Task SaveAsync(Stream stream, string fullPath, CancellationToken token = default)
        {
            if(stream.Length == 0 || stream == null)
                throw new ArgumentNullException(nameof(stream), "File data cannot be null or empty");

            var pathToDir = Path.GetDirectoryName(fullPath);
            var dirInfo = new DirectoryInfo(pathToDir);
            if (!dirInfo.Exists)
                dirInfo.Create();

            using FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);

            stream.Position = 0;
            await stream.CopyToAsync(fs, token);
            
        }
        public bool FileExists(string fullPath)
        {
            return File.Exists(fullPath);
        }
        public async Task DeleteAsync(string fullPath, CancellationToken token = default)
        {
            if (!FileExists(fullPath))
                return;

            int numberOfAttempts = 3;
            int delay = 200;

            for (int attemp = 0; attemp < numberOfAttempts; attemp++)
                try
                {
                    File.Delete(fullPath);
                }
                catch (IOException ex) when (attemp < numberOfAttempts)
                {
                    await Task.Delay(delay, token);
                }
                catch (Exception ex)
                {
                    throw;
                }
        }
    }
}
