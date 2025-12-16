using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using AdsManagement.Tests.FakeData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdsManagement.Tests.AdvertisementT
{
    public class AdvertisementStorageTests
    {
        [Fact]
        public async Task GetAndAddAsync_Test()
        {
            //Arrange
            string nameDataBase = "AddAndGetDb001";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
                "Производительность , безопасность", Guid.NewGuid());
            var ad2 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак", Guid.NewGuid());
            var ad3 = new Advertisement(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"), "Создаем цифровые сервисы", 3,
                "Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", Guid.NewGuid());

            var ad4 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Создаем цифровые сервисы", 3,
               "Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", Guid.NewGuid());

            //Act - Add

            var result1 = await storage.AddAsync(ad1);
            var result2 = await storage.AddAsync(ad2);

            //Asser + act
            await Assert.ThrowsAsync<ExceedingTheAdLimitException>(async () => await storage.AddAsync(ad4));

            var result3 = await storage.AddAsync(ad3);

            //Act - Get
            var adDb1 = await storage.GetAsync(ad1.Id);
            var adDb3 = await storage.GetAsync(ad3.Id);
            var adDbNull = await storage.GetAsync(Guid.Parse("7c1f8a4e-3b62-4e9d-9a2f-5c6d8e1b0a34"));


            //Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);

            Assert.Equal("IT решения", adDb1.Title);
            Assert.Equal("DEX", adDb3.User.Name);
            Assert.Null(adDbNull);

        }
        [Fact]
        public async Task DeleteAsync_Test()
        {
            //Arrange
            string nameDataBase = "DeleteDb001";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
                "Производительность , безопасность", Guid.NewGuid());

            var resultAdd = await storage.AddAsync(ad1);

            //Act
            var resultDel = await storage.DeleteAsync(ad1.Id);

            //Assert + Act
            await Assert.ThrowsAsync<AdvertisementNotFoundException>(async () => 
            await storage.DeleteAsync(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34")));

            //Assert
            Assert.True(resultAdd);
            Assert.True(resultDel);
        }
        [Fact]
        public async Task UpdateAsync_Test()
        {
            //Arrange
            string nameDataBase = "UpDb001";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);
            var testId = Guid.NewGuid();

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак", testId);

            var ad1Up = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Cybersecurity", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак", testId);

            await storage.AddAsync(ad1);

            //Act
            var result = await storage.UpdateAsync(ad1Up);
            var resultFalse = await storage.UpdateAsync(null);

            //Assert
            Assert.True(result);
            Assert.False(resultFalse);
        }

        [Fact]
        public async Task GetFilterAdsAsync_Test()
        {
            //Arrange
            string nameDataBase = "FilterDb001";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
               "Производительность , безопасность", new DateTime(2025, 06, 11), new DateTime(2025, 8, 11));

            var ad2 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак", 
                date.UtcNow.AddMonths(-1), date.UtcNow.AddDays(1));

            var ad3 = new Advertisement(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"), "Создаем цифровые сервисы", 3,
                "Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB" +
                " и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            var ad4 = new Advertisement(Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"), "IT | DNS ПК и ноутбуки", 2,
               "Топовые игровые ноутбуки и пк", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            await storage.AddAsync(ad1);
            await storage.AddAsync(ad2);
            await storage.AddAsync(ad3);
            await storage.AddAsync(ad4);

            AdFilterDto filterDto = new AdFilterDto()
            {
                Title = "IT",
                SortBy = "Number",
                SortDesc = true,
                Page = 0,
                PageSize = 0
                
            };

            //Act
            var result = await storage.GetFilterAdsAsync(filterDto);

            //Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal("IT | DNS ПК и ноутбуки", result.Items[0].Title);
            Assert.Equal("IT решения", result.Items[1].Title);
        }
        [Fact]
        public async Task GetUserAdsCountAll_And_OnlyActive_Test()
        {
            //Arrange
            string nameDataBase = "CountAllDb010";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
              "Производительность , безопасность", new DateTime(2025, 06, 11), new DateTime(2025, 8, 11));

            var ad2 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак",
                date.UtcNow.AddMonths(-1), date.UtcNow.AddDays(1));

            var ad3 = new Advertisement(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"), "Создаем цифровые сервисы", 3,
                "Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB" +
                " и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            var ad4 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT | DNS ПК и ноутбуки", 2,
               "Топовые игровые ноутбуки и пк", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            await storage.AddAsync(ad1);
            await storage.AddAsync(ad2);
            await storage.AddAsync(ad3);
            await storage.AddAsync(ad4);

            //Act
            var resultAll = await storage.GetUserAdsCountAll(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));
            var resultActive = await storage.GetUserAdsCountActive(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));
            var result0 = await storage.GetUserAdsCountActive(Guid.Parse("5d9f3e2a-7c6b-4a18-8e1f-b2c0d4a9e756"));

            //Assert
            Assert.Equal(3, resultAll);
            Assert.Equal(2, resultActive);
            Assert.Equal(0, result0);
        }
        [Fact]
        public async Task GetUserAdsAsync_Test()
        {
            //Arrange
            string nameDataBase = "CountAllDb001";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDataBase)
                .Options;

            using var dbContext = new AdsDbContext(options);

            FakeDateTimeProvider date = new FakeDateTimeProvider();
            date.UtcNow = new DateTime(2025, 12, 14);
            int adLimit = 2;
            var storage = new AdvertisementStorage(dbContext, date, adLimit);

            await SetDataTest(nameDataBase);

            var ad1 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT решения", 1,
              "Производительность , безопасность", new DateTime(2025, 06, 11), new DateTime(2025, 8, 11));

            var ad2 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Кибербезопасность для ваших систем", 2,
                "Безопасность - это наше все. Предотвратили более 5000 кибератак",
                date.UtcNow.AddMonths(-1), date.UtcNow.AddDays(1));

            var ad3 = new Advertisement(Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"), "Создаем цифровые сервисы", 3,
                "Полный цикл разработки IT-продуктов — от идеи до реализации.РАЗРАБОТКА|WEB" +
                " и Mobile для B2B и B2C, Enterprise-решения Highload-класса.", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            var ad4 = new Advertisement(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "IT | DNS ПК и ноутбуки", 2,
               "Топовые игровые ноутбуки и пк", date.UtcNow.AddMonths(-1), date.UtcNow.AddMonths(1));

            await storage.AddAsync(ad1);
            await storage.AddAsync(ad2);
            await storage.AddAsync(ad3);
            await storage.AddAsync(ad4);

            UserAdvertisementFilterDto filter = new UserAdvertisementFilterDto()
            {
                IsExpired = false,
                Page = 0,
                PageSize = 0
            };

            //Act
            var result = await storage.GetUserAdsAsync(Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), filter);

            //Assert + Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await storage.GetUserAdsAsync(Guid.Parse("00000000-0000-0000-0000-000000000000"), filter));
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await storage.GetUserAdsAsync(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), filter));

            //Assert
            Assert.Equal(2, result.TotalCount);
        }

        private async Task SetDataTest(string nameDataBase)
        {
            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(nameDataBase)
               .Options;

            using var dbContext = new AdsDbContext(options);

            var storageUser = new UserStorage(dbContext);
            var storageRole = new RoleStorage(dbContext);

            Role role = new Role("User", Guid.NewGuid());

            List<User> users = new List<User>
            {
                new User("Test0", role.Id, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91")),
                new User("Test1", role.Id, Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88")),
                new User("Test2", role.Id,  Guid.Parse("a6d2c4f1-1e59-4b92-8f7e-6c3a9d0e5b21")),
                new User("DEX", role.Id,  Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34")),
                new User("Test4", role.Id,  Guid.Parse("5d9f3e2a-7c6b-4a18-8e1f-b2c0d4a9e756"))
            };

            await storageRole.AddAsync(role);

            foreach (var entity in users)
            {
                await storageUser.AddAsync(entity);
            }
        }
    }
}
