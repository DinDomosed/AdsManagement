using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using AdsManagement.Tests.FakeData;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdsManagement.Tests.AdvertisementImages
{
    public class AdImageStorageTests
    {
        [Fact]
        public async Task AddAsync_Test()
        {
            //Arrange 
            string nameDataBase = "ImageDbAdd";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
          .UseInMemoryDatabase(nameDataBase)
          .Options;

            using var dbContext = new AdsDbContext(options);
            var storage = new AdImageStorage(dbContext);

            var image1 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original1", "file/small1");
            var image2 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original2", "file/small2");
            var image3 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original3", "file/small3");
            var imageWithAdNoExists = new AdvertisementImage(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a39"), "file/original3", "file/small3");
            AdvertisementImage imageNull = null;

            await SetDataTest(nameDataBase);

            //Act
            var resultId = await storage.AddAsync(image1);
            await storage.AddAsync(image2);

            //Assert + Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await storage.AddAsync(imageNull));
           
            await Assert.ThrowsAsync<AdvertisementNotFoundException>(async () => await storage.AddAsync(imageWithAdNoExists));

            //Assert
            Assert.NotEqual(Guid.Empty, resultId);
        }
        [Fact]
        public async Task DeleteAsync_Test()
        {
            //Arrange 
            string nameDataBase = "ImageDbDel";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
          .UseInMemoryDatabase(nameDataBase)
          .Options;

            using var dbContext = new AdsDbContext(options);
            var storage = new AdImageStorage(dbContext);

            var image1 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original1", "file/small1");
            var image2 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original2", "file/small2");

            await SetDataTest(nameDataBase);

            await storage.AddAsync(image1);
            Guid dbId = await storage.AddAsync(image2);

            //Act
            await storage.DeleteAsync(dbId);

            //Assert + Act
            await Assert.ThrowsAsync<AdImageNotFoundException>(async () =>await  storage.GetAsync(dbId));
            await Assert.ThrowsAsync<ArgumentException>(async () => await storage.DeleteAsync(Guid.Empty));
            await Assert.ThrowsAsync<AdImageNotFoundException>(async () => await storage.DeleteAsync(Guid.NewGuid()));

            Assert.NotEqual(Guid.Empty, dbId);
        }
        [Fact]
        public async Task GetAsync_Test()
        {
            //Arrange 
            string nameDataBase = "ImageDbGet";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
          .UseInMemoryDatabase(nameDataBase)
          .Options;

            using var dbContext = new AdsDbContext(options);
            var storage = new AdImageStorage(dbContext);

            var image1 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original1", "file/small1");
            var image2 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original2", "file/small2");

            await SetDataTest(nameDataBase);

            await storage.AddAsync(image1);
            Guid dbId = await storage.AddAsync(image2);

            //Act
            var dbImage = await storage.GetAsync(dbId);

            //Assert + Act
            await Assert.ThrowsAsync<AdImageNotFoundException>(async () => await storage.GetAsync(Guid.NewGuid()));

            //Assert
            Assert.NotNull(dbImage);
            Assert.Equal(image2.OriginalImagePath, dbImage.OriginalImagePath);
        }
        [Fact]
        public async Task GetByAdIdAsync_Test()
        {
            //Arrange 
            string nameDataBase = "ImageDbGetByIdAd";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
          .UseInMemoryDatabase(nameDataBase)
          .Options;

            using var dbContext = new AdsDbContext(options);
            var storage = new AdImageStorage(dbContext);

            var image1 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original1", "file/small1");
            var image2 = new AdvertisementImage(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), "file/original2", "file/small2");

            await SetDataTest(nameDataBase);

            await storage.AddAsync(image1);
            await storage.AddAsync(image2);

            //Act
            var images = await storage.GetByAdIdAsync(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"));

            //Assert + Act
            await Assert.ThrowsAsync<AdvertisementNotFoundException>(async () => await storage.GetByAdIdAsync(Guid.NewGuid()));

            //Assert
            Assert.Equal(2, images.Count);
        }
        private async Task SetDataTest(string nameDataBase)
        {
            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(nameDataBase)
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
                new User("Test2", role.Id,  Guid.Parse("a6d2c4f1-1e59-4b92-8f7e-6c3a9d0e5b21")),
                new User("DEX", role.Id,  Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34")),
                new User("Test4", role.Id,  Guid.Parse("5d9f3e2a-7c6b-4a18-8e1f-b2c0d4a9e756"))
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
