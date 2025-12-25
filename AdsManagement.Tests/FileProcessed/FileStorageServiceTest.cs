using AdsManagement.App.Interfaces;
using AdsManagement.FileStorage;
using Xunit;

namespace AdsManagement.Tests.FileProcessed
{
    public class FileStorageServiceTest
    {
        [Fact]
        public async Task SaveAsync_Test()
        {
            //Arrange
            IFileStorageService storageService = new FileStorageService();
            var path = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            var bytes = await File.ReadAllBytesAsync(path);

            var pathMSi = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");
            var bytesMsi = await File.ReadAllBytesAsync(pathMSi);

            var pathlogo = Path.Combine("FakeData", "TestImages", "accountlogo.jpg");
            var byteslogo = await File.ReadAllBytesAsync(pathlogo);

            int count = 1;


            var finalPath = Path.Combine("D:", "AdsImages", $"image{count}.jpg");
            var finalPathMSi = Path.Combine("D:", "AdsImages", $"imageMSI{count}.jpg");
            var finalPathLogo = Path.Combine("D:", "AdsImages", $"imageLogo{count}.jpg");

            //Act
            await storageService.SaveAsync(bytes, finalPath);
            await storageService.SaveAsync(bytesMsi, finalPathMSi);
            await storageService.SaveAsync(byteslogo, finalPathLogo);

            //Assert
            Assert.True(File.Exists(finalPath));
            Assert.True(File.Exists(finalPathMSi));
            Assert.True(File.Exists(finalPathLogo));
        }
        [Fact]
        public async Task DeleteAsync_Test ()
        {
            IFileStorageService storageService = new FileStorageService();
            var path = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");
            var bytes = await File.ReadAllBytesAsync(path);
            int count = 2;

            var finalPath = Path.Combine("D:", "AdsImages", $"imageMSi{count}.jpg");
            await storageService.SaveAsync(bytes, finalPath);

            //Act
            bool resuktSave = File.Exists(finalPath);
            await storageService.DeleteAsync(finalPath);

            //Assert
            Assert.False(File.Exists(finalPath));
        }
        [Fact]
        public async Task SaveAsync_TestStream()
        {
            //Arrange
            IFileStorageService storageService = new FileStorageService();

            
            var path = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            var pathMSi = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");
            var pathlogo = Path.Combine("FakeData", "TestImages", "accountlogo.jpg");

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fsMsi = new FileStream(pathMSi, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fsLogo = new FileStream(pathlogo, FileMode.Open, FileAccess.Read, FileShare.Read);

            int count = 3;

            var finalPath = Path.Combine("D:", "AdsImages", $"image{count}.jpg");
            var finalPathMSi = Path.Combine("D:", "AdsImages", $"imageMSI{count}.jpg");
            var finalPathLogo = Path.Combine("D:", "AdsImages", $"imageLogo{count}.jpg");

            //Act
            await storageService.SaveAsync(fs, finalPath);
            await storageService.SaveAsync(fsMsi, finalPathMSi);
            await storageService.SaveAsync(fsLogo, finalPathLogo);

            //Assert
            Assert.True(File.Exists(finalPath));
            Assert.True(File.Exists(finalPathMSi));
            Assert.True(File.Exists(finalPathLogo));
        }
    }
}
