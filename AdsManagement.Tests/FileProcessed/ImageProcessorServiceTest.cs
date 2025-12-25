using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Services;
using Xunit;

namespace AdsManagement.Tests.FileProcessed
{
    public class ImageProcessorServiceTest
    {
        [Fact]
        public async Task СompressionImageAsync_Test()
        {
            //Arrange
            IImageProcessorService imageProcessor = new ImageProcessorService();

            var path = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            var pathMSi = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");
            var pathlogo = Path.Combine("FakeData", "TestImages", "accountlogo.jpg");

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fsMsi = new FileStream(pathMSi, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var fsLogo = new FileStream(pathlogo, FileMode.Open, FileAccess.Read, FileShare.Read);

            //Act
            using var stream1 = await  imageProcessor.СompressionImageAsync(fs);
            using var streamMSi = await  imageProcessor.СompressionImageAsync(fsMsi);
            using var streamLogo = await  imageProcessor.СompressionImageAsync(fsLogo);

            //Assert
            Assert.NotEqual(fs.Length, stream1.Length);
            Assert.True(fs.Length > stream1.Length);
            Assert.True(fsMsi.Length > streamMSi.Length);
            Assert.True(fsLogo.Length > streamLogo.Length);
        }
    }
}
