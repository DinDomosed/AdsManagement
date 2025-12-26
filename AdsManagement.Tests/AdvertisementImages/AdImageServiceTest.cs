using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Mappings;
using AdsManagement.App.Services;
using AdsManagement.App.Settings;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using AdsManagement.FileStorage;
using AdsManagement.Tests.FakeData;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AdsManagement.Tests.AdvertisementImages
{
    public class AdImageServiceTest
    {
        [Fact]
        public async Task GetByAdIdAsync_Test()
        {
            //Arrange 
            string dataBaseName = "AdImageServiceDBGet";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<AdImageProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();
            using var dbContext = new AdsDbContext(options);

            SetDataTest(dataBaseName);

            AdImageServiceSettings settings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };
            IOptions<AdImageServiceSettings> option = Options.Create(settings);

            IImageProcessorService imageProcessor = new ImageProcessorService();
            IFileStorageService fileStorageService = new FileStorageService();

            IAdvertisementStorage storage = new AdvertisementStorage(dbContext,
                new FakeDateTimeProvider() { UtcNow = DateTime.UtcNow });

            IAccessValidationsService accessValidations = new AccessValidationsService(storage, default);

            IAdImageStorage imageStorage = new AdImageStorage(dbContext);
            IAdImageService service = new AdImageService(imageStorage, option, mapper, imageProcessor, fileStorageService, accessValidations);

            var image1 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original1", "file/small1");

            var dbId = await imageStorage.AddAsync(image1);

            //Act

            var dbImage = await service.GetByAdIdAsync(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"));

            //Assert
            Assert.Equal(dbImage[0].OriginalImagePath, image1.OriginalImagePath);
            Assert.Equal(dbImage[0].SmallImagePath, image1.SmallImagePath);
        }
        [Fact]
        public async Task AddAdImageAsync_Test()
        {
            //Arrange 
            string dataBaseName = "AdImageServiceDBAdd";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<AdImageProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();
            using var dbContext = new AdsDbContext(options);

            SetDataTest(dataBaseName);

            AdImageServiceSettings settings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 1,
                MaximumSizeMb = 4
            };
            IOptions<AdImageServiceSettings> option = Options.Create(settings);

            IImageProcessorService imageProcessor = new ImageProcessorService();
            IFileStorageService fileStorageService = new FileStorageService();

            IAdvertisementStorage storage = new AdvertisementStorage(dbContext,
                new FakeDateTimeProvider() { UtcNow = DateTime.UtcNow });

            IAccessValidationsService accessValidations = new AccessValidationsService(storage, default);

            IAdImageStorage imageStorage = new AdImageStorage(dbContext);
            IAdImageService service = new AdImageService(imageStorage, option, mapper, imageProcessor, fileStorageService, accessValidations);

            var fullPathToImage1 = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            var fullPathToImage2 = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");
            var fullPathToImage3 = Path.Combine("FakeData", "TestImages", "accountlogo.jpg");

            using FileStream fs = new FileStream(fullPathToImage1, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream fs2 = new FileStream(fullPathToImage2, FileMode.Open, FileAccess.Read, FileShare.Read);
            using FileStream fs3 = new FileStream(fullPathToImage3, FileMode.Open, FileAccess.Read, FileShare.Read);

            FileDataFake file1 = new FileDataFake(fs, "image/png");
            FileDataFake file2 = new FileDataFake(fs2, "image/webp");
            FileDataFake file3 = new FileDataFake(fs3, "image/jpeg");


            //Act
            var imageId_1 = await service.AddAdImageAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), file1,
                Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));

            var imageDb_1 = await service.GetImageAsync(imageId_1);

            bool result1 = File.Exists(imageDb_1.SmallImagePath) && File.Exists(imageDb_1.OriginalImagePath);
            bool resultSize = new FileInfo(imageDb_1.SmallImagePath).Length < new FileInfo(imageDb_1.OriginalImagePath).Length;

            //Assert + Act

            await Assert.ThrowsAsync<AdvertisementImageLimitExceededException>(async () =>
            await service.AddAdImageAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), file2, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91")));

            await Assert.ThrowsAsync<AccessDeniedException>(async () =>
            await service.AddAdImageAsync(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), file2, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a96")));// этот падает 

            await Assert.ThrowsAsync<InvalidFileWeightException>(async () =>
            await service.AddAdImageAsync(Guid.Parse("b2d4f6a1-8c3e-4b9a-92d1-6e5f7a0c4b83"), file3, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91")));

            //Assert
            Assert.True(result1);
            Assert.True(resultSize);
            Assert.NotNull(imageDb_1);

        }
        [Fact]
        public async Task DeleteAdImageAsync_Test()
        {
            //Arrange 
            string dataBaseName = "AdImageServiceDBDel";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<AdImageProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();
            using var dbContext = new AdsDbContext(options);

            SetDataTest(dataBaseName);

            AdImageServiceSettings settings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 1,
                MaximumSizeMb = 4
            };
            IOptions<AdImageServiceSettings> option = Options.Create(settings);

            IImageProcessorService imageProcessor = new ImageProcessorService();
            IFileStorageService fileStorageService = new FileStorageService();

            IAdvertisementStorage storage = new AdvertisementStorage(dbContext,
                new FakeDateTimeProvider() { UtcNow = DateTime.UtcNow });

            IAccessValidationsService accessValidations = new AccessValidationsService(storage, default);

            IAdImageStorage imageStorage = new AdImageStorage(dbContext);
            IAdImageService service = new AdImageService(imageStorage, option, mapper, imageProcessor, fileStorageService, accessValidations);

            var fullPathToImage1 = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            using FileStream fs = new FileStream(fullPathToImage1, FileMode.Open, FileAccess.Read, FileShare.Read);
            FileDataFake file1 = new FileDataFake(fs, "image/png");

            var imageId_1 = await service.AddAdImageAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), file1,
               Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));

            var imageDb_1 = await service.GetImageAsync(imageId_1);
            bool filesExists = File.Exists(imageDb_1.SmallImagePath) && File.Exists(imageDb_1.OriginalImagePath);

            //Act + Assert 
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.DeleteAdImageAsync(imageId_1, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a99")));

            //Act

            await service.DeleteAdImageAsync(imageId_1, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));
            bool fileNoExistsBeforDelete = !(File.Exists(imageDb_1.SmallImagePath) && File.Exists(imageDb_1.OriginalImagePath));

            //Assert
            Assert.True(filesExists);
            Assert.True(fileNoExistsBeforDelete);

        }
        private async Task SetDataTest(string dataBaseName)
        {

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);

            var storageUser = new UserStorage(dbContext);
            var storageRole = new RoleStorage(dbContext);
            var adStorage = new AdvertisementStorage(dbContext, date);


            Role role = new Role("User", Guid.NewGuid());

            List<User> users = new List<User>
            {
                new User("Test0", role.Id, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91")),
                new User("Test1", role.Id, Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88")),
                new User("DEX", role.Id,  Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"))
            };

            //Объявления для текстов
            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
              "Производительность , безопасность", Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"));

            var ad2 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак",
                Guid.Parse("b2d4f6a1-8c3e-4b9a-92d1-6e5f7a0c4b83"));

            var ad3 = new Advertisement(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"), "Создаем цифровые сервисы", 3,
                "DEX Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB" +
                " и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"));

            //Добавление сущностей в бд
            await storageRole.AddAsync(role);
            foreach (var entity in users)
            {
                await storageUser.AddAsync(entity);
            }

            await adStorage.AddAsync(ad1);
            await adStorage.AddAsync(ad2);
            await adStorage.AddAsync(ad3);
        }
    }
}
