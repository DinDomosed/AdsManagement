using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.DTOs.Comment;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Service;
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

namespace AdsManagement.Tests.Advertisements
{
    public class AdvertisementServiceTest
    {
        IMapper mapper;
        [Fact]
        public async Task AddAdvertisementAsync_Test()
        {
            //Arrange 
            string nameDataBase = "AdServiceAddDb";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
              .UseInMemoryDatabase(nameDataBase)
              .Options;

            using var dbContext = new AdsDbContext(options);

            var imageSettings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };

            var adSettngs = new AdServiceSettings()
            {
                LimitAdvertisement = 2
            };

            await SetDataTest(nameDataBase);
            IOptions<AdImageServiceSettings> optionsS = Options.Create(imageSettings);
            IOptions<AdServiceSettings> optoinsAd = Options.Create(adSettngs);

            IDateTimeProvider date = new FakeDateTimeProvider();
            var imageStorage = new AdImageStorage(dbContext);
            var storage = new AdvertisementStorage(dbContext, date);

            IAccessValidationsService access = new AccessValidationsService(storage, default);

            var imageService = new AdImageService(imageStorage, optionsS, mapper, new ImageProcessorService(), new FileStorageService(), access);
            var service = new AdvertisementService(storage, mapper, imageService, new FileStorageService(), optoinsAd, access);

            var testImgaePath = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            var testImgaePath2 = Path.Combine("FakeData", "TestImages", "accountlogo.jpg");
            var testImgaePath3 = Path.Combine("FakeData", "TestImages", "msi_dragon.webp");

            using var stream = new FileStream(testImgaePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var stream2 = new FileStream(testImgaePath2, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var stream3 = new FileStream(testImgaePath3, FileMode.Open, FileAccess.Read, FileShare.Read);
            IFileData fileData1 = new FileDataFake(stream, "image/png");
            IFileData fileData2 = new FileDataFake(stream2, "image/jpg");
            IFileData fileData3 = new FileDataFake(stream3, "image/webp");

            CreateAdvertisementDto ad1 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Мир компьютеров",
                Text = "Лучшие пк и ноутбуки"
            };
            CreateAdvertisementDto ad2 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Хайтек",
                Text = "Продажа электроники и бытовой техники в ПМР"
            };

            //Act

            var Id1 = await service.AddAdvertisementAsync(ad1, fileData1);
            var Id2 = await service.AddAdvertisementAsync(ad2, fileData3);

            var adDb = await storage.GetAsync(Id1);
            var adDb2 = await storage.GetAsync(Id2);

            //Act + Assert
            await Assert.ThrowsAsync<ExceedingTheAdLimitException>(async () => await service.AddAdvertisementAsync(ad1, fileData1));

            //Assert
            Assert.Equal(adDb.Title, ad1.Title);
            Assert.Equal(adDb.Text, ad1.Text);
            Assert.Equal(adDb.UserId, ad1.UserId);
            Assert.Equal(adDb.Number, 1);
            Assert.Equal(adDb2.Number, 2);

        }
        [Fact]
        public async Task UpdateAdvertisementAsync_Test()
        {
            //Arrange 

            string nameDataBase = "AdServiceUpdateDb";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
              .UseInMemoryDatabase(nameDataBase)
              .Options;

            var dbContext = new AdsDbContext(options);

            var imageSettings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };

            var adSettngs = new AdServiceSettings()
            {
                LimitAdvertisement = 2
            };

            await SetDataTest(nameDataBase);
            IOptions<AdImageServiceSettings> optionsS = Options.Create(imageSettings);
            IOptions<AdServiceSettings> optoinsAd = Options.Create(adSettngs);

            IDateTimeProvider date = new FakeDateTimeProvider();
            var imageStorage = new AdImageStorage(dbContext);
            var storage = new AdvertisementStorage(dbContext, date);

            IAccessValidationsService access = new AccessValidationsService(storage, default);

            var imageService = new AdImageService(imageStorage, optionsS, mapper, new ImageProcessorService(), new FileStorageService(), access);
            var service = new AdvertisementService(storage, mapper, imageService, new FileStorageService(), optoinsAd, access);

            CreateAdvertisementDto ad1 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Мир компьютеров",
                Text = "Лучшие пк и ноутбуки"
            };
            CreateAdvertisementDto ad2 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Хайтек",
                Text = "Продажа электроники и бытовой техники в ПМР"
            };

            var dbId1 = await service.AddAdvertisementAsync(ad1);
            var dbId2 = await service.AddAdvertisementAsync(ad2);

            var adDbBefore1 = await service.GetAdvertisementAsync(dbId1);
            var adDbBefore2 = await service.GetAdvertisementAsync(dbId2);

            UpdateAdvertisementDto upDto1 = new UpdateAdvertisementDto
            {
                Id = dbId1,
                Text = "Техника мирового уровня, лучшие пк и ноутбуки",
                Title = "Мир компьютеров"
            };
            UpdateAdvertisementDto upDto2 = new UpdateAdvertisementDto
            {
                Id = dbId2,
                Text = "Продажа электроники и бытовой техники в ПМР",
                Title = "HItech"
            };

            //Act

            await service.UpdateAdvertisementAsync(upDto1, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));
            await service.UpdateAdvertisementAsync(upDto2, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));

            var dbAd1 = await service.GetAdvertisementAsync(dbId1);
            var dbAd2 = await service.GetAdvertisementAsync(dbId2);

            //Act + Assert
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.UpdateAdvertisementAsync(upDto2, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a98")));

            //Assert
            Assert.Equal(dbAd1.CreatedAt, adDbBefore1.CreatedAt);
            Assert.Equal(dbAd1.ExpiresAt, adDbBefore1.ExpiresAt);
            Assert.Equal(dbAd1.Number, adDbBefore1.Number);
            Assert.Equal(dbAd1.Rating, adDbBefore1.Rating);
            Assert.Equal(dbAd1.UserId, adDbBefore1.UserId);
            Assert.Equal(dbAd1.Text, upDto1.Text);
            Assert.Equal(dbAd1.Title, upDto1.Title);
            Assert.NotEqual(dbAd1.Text, adDbBefore1.Text);

        }
        [Fact]
        public async Task DeleteAdvertisementAsync_Test()
        {
            //Arrange 
            string nameDataBase = "AdServiceDeleteDb";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
              .UseInMemoryDatabase(nameDataBase)
              .Options;

            var dbContext = new AdsDbContext(options);

            var imageSettings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };

            var adSettngs = new AdServiceSettings()
            {
                LimitAdvertisement = 2
            };

            await SetDataTest(nameDataBase);
            IOptions<AdImageServiceSettings> optionsS = Options.Create(imageSettings);
            IOptions<AdServiceSettings> optoinsAd = Options.Create(adSettngs);

            IDateTimeProvider date = new FakeDateTimeProvider();
            var imageStorage = new AdImageStorage(dbContext);
            var storage = new AdvertisementStorage(dbContext, date);

            IAccessValidationsService access = new AccessValidationsService(storage, default);

            var imageService = new AdImageService(imageStorage, optionsS, mapper, new ImageProcessorService(), new FileStorageService(), access);
            var service = new AdvertisementService(storage, mapper, imageService, new FileStorageService(), optoinsAd, access);

            CreateAdvertisementDto ad1 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Мир компьютеров",
                Text = "Лучшие пк и ноутбуки"
            };

            var originalPathImage = Path.Combine("FakeData", "TestImages", "dream_machine.png");
            using FileStream fs = new FileStream(originalPathImage, FileMode.Open, FileAccess.Read, FileShare.Read);

            IFileData fileData = new FileDataFake(fs, "image/png");

            var dbId1 = await service.AddAdvertisementAsync(ad1, fileData);

            var dbAds = await service.GetAdvertisementAsync(dbId1);

            var fileExists = File.Exists(dbAds.Images.First().OriginalImagePath);
            var fileExistsSmall = File.Exists(dbAds.Images.First().SmallImagePath);

            //Act

            await service.DeleteAdvertisementAsync(dbId1, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));

            var fileNotExists = !File.Exists(dbAds.Images.First().OriginalImagePath);
            var fileNotExistsSmall = !File.Exists(dbAds.Images.First().SmallImagePath);

            //Assert
            Assert.True(fileExists);
            Assert.True(fileExistsSmall);
            Assert.True(fileNotExists);
            Assert.True(fileNotExistsSmall);
        }
        [Fact]
        public async Task GetFilterAdsAsync_Test()
        {
            //Arrange 
            string nameDataBase = "AdServiceFilterDb";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
              .UseInMemoryDatabase(nameDataBase)
              .Options;

            var dbContext = new AdsDbContext(options);

            var imageSettings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };

            var adSettngs = new AdServiceSettings()
            {
                LimitAdvertisement = 2
            };

            await SetDataTest(nameDataBase);
            IOptions<AdImageServiceSettings> optionsS = Options.Create(imageSettings);
            IOptions<AdServiceSettings> optoinsAd = Options.Create(adSettngs);

            IDateTimeProvider date = new FakeDateTimeProvider();
            var imageStorage = new AdImageStorage(dbContext);
            var storage = new AdvertisementStorage(dbContext, date);

            IAccessValidationsService access = new AccessValidationsService(storage, default);

            var imageService = new AdImageService(imageStorage, optionsS, mapper, new ImageProcessorService(), new FileStorageService(), access);
            var service = new AdvertisementService(storage, mapper, imageService, new FileStorageService(), optoinsAd, access);

            CreateAdvertisementDto ad1 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Мир компьютеров",
                Text = "Лучшие пк и ноутбуки"
            };
            CreateAdvertisementDto ad2 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                Title = "Компьютерный дом",
                Text = "Ремонт пк и ноутбуков"
            };
            CreateAdvertisementDto ad3 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                Title = "Мир компьютеров",
                Text = "hhhhhhhhh"
            };
            CreateAdvertisementDto ad4 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Хайтек",
                Text = "Бытовая техника для семьи"
            };

            await service.AddAdvertisementAsync(ad1);
            await service.AddAdvertisementAsync(ad2);
            await service.AddAdvertisementAsync(ad3);
            await service.AddAdvertisementAsync(ad4);

            var filter = new AdFilterDto()
            {
                Title = "Мир",
                SortBy = "Title"
            };

            //Act

            var result = await service.GetFilterAdsAsync(filter);

            //Assert
            Assert.Equal(2, result.TotalCount);
        }
        [Fact]
        public async Task RecalculateRatingAsync_Test()
        {
            //Arrange 
            string nameDataBase = "AdServiceRecalculateDb";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
              .UseInMemoryDatabase(nameDataBase)
              .Options;

            var dbContext = new AdsDbContext(options);

            var imageSettings = new AdImageServiceSettings()
            {
                ImagesDirectory = Path.Combine("D:", "AdsImages"),
                LimitImage = 2,
                MaximumSizeMb = 4
            };

            var adSettngs = new AdServiceSettings()
            {
                LimitAdvertisement = 2
            };

            await SetDataTest(nameDataBase);
            IOptions<AdImageServiceSettings> optionsS = Options.Create(imageSettings);
            IOptions<AdServiceSettings> optoinsAd = Options.Create(adSettngs);

            IDateTimeProvider date = new FakeDateTimeProvider();
            var imageStorage = new AdImageStorage(dbContext);
            var storage = new AdvertisementStorage(dbContext, date);

            var commentStorage = new CommentStorage(dbContext);
            IAccessValidationsService access = new AccessValidationsService(storage, commentStorage);

            var imageService = new AdImageService(imageStorage, optionsS, mapper, new ImageProcessorService(), new FileStorageService(), access);
            var service = new AdvertisementService(storage, mapper, imageService, new FileStorageService(), optoinsAd, access);

            CreateAdvertisementDto ad1 = new CreateAdvertisementDto
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                Title = "Мир компьютеров",
                Text = "Лучшие пк и ноутбуки"
            };

            var adId = await service.AddAdvertisementAsync(ad1);

            var commentService = new CommentService(commentStorage, mapper, access);

            commentService.CommentEstinationChanged += service.RecalculateRatingAsync;

            var comments = new List<CreateCommentDto>
            {
                new CreateCommentDto()
            {
                AdvertisementId = adId,
                Estimation = 5,
                Text ="отлично ",
                UserId =  Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88")
            },
                 new CreateCommentDto()
            {
                AdvertisementId = adId,
                Estimation = 4,
                Text ="хорошо ",
                 UserId = Guid.Parse("a6d2c4f1-1e59-4b92-8f7e-6c3a9d0e5b21")
            },
                  new CreateCommentDto()
            {
                AdvertisementId = adId,
                Estimation = 3,
                Text ="средне ",
                UserId = Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34")

            }
            };

            await service.AddAdvertisementAsync(ad1);
            var adBefore = await service.GetAdvertisementAsync(adId);

            var expectedRating = comments.Average(c => (decimal?)c.Estimation);

            //Act
            var tasks = comments.Select(async comment =>
            await commentService.AddCommentAsync(comment));

            await Task.WhenAll(tasks);

            var adAfter = await service.GetAdvertisementAsync(adId);

            //Assert
            Assert.Equal(0, adBefore.Rating);
            Assert.Equal(expectedRating, adAfter.Rating);

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

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<AdvertisementProfile>();
            configExpression.AddProfile<AdImageProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<CommentProfile>();
            var config = new MapperConfiguration(configExpression, new NullLoggerFactory());
            mapper = config.CreateMapper();
        }
    }
}
