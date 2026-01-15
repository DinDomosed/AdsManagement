using AdsManagement.App.DTOs.Comment;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Events;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Service.Events;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Mappings;
using AdsManagement.App.Services;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using AdsManagement.Tests.FakeData;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AdsManagement.Tests.Comments
{
    public class CommentServiceTest
    {
        [Fact]
        public async Task AddCommentAsync_And_Get_Test()
        {
            //Arrange
            string dataBaseName = "CommentServiceDBAdd";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<CommentProfile>();
            configExpression.AddProfile<PagedResultProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            ICommentStorage storage = new CommentStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, storage);
            ICommentEventsDispatcher dispatcher = new CommentEventsDispatcher(new List<ICommentEstimationChangedHandler>());
            ICommentService service = new CommentService(storage, mapper, accessValidations, dispatcher);

            var comment1 = new CreateCommentDto()
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Супер пупер"
            };
            var comment1_False = new CreateCommentDto()
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 5,
                Text = "Отлично"
            };
            var comment2 = new CreateCommentDto()
            {
                UserId = Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Класс"
            };

            SetDataTest(dataBaseName);

            //Act
            var result1 = await service.AddCommentAsync(comment1);
            var result2 = await service.AddCommentAsync(comment2);
            var resultGet1 = await service.GetByAdvertisementAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"));

            //Assert + Act 
            await Assert.ThrowsAsync<CommentAlreadyExistsException>(async () => await service.AddCommentAsync(comment1_False));

            //Assert
            Assert.NotEqual(Guid.Empty, result1);
            Assert.NotEqual(Guid.Empty, result2);
            Assert.Equal(2, resultGet1.TotalCount);
        }
        [Fact]
        public async Task DeleteCommentAsync_Test()
        {
            //Arrange
            string dataBaseName = "CommentServiceDBDel";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<CommentProfile>();
            configExpression.AddProfile<PagedResultProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            ICommentStorage storage = new CommentStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, storage);
            ICommentEventsDispatcher dispatcher = new CommentEventsDispatcher(new List<ICommentEstimationChangedHandler>());
            ICommentService service = new CommentService(storage, mapper, accessValidations, dispatcher);

            var comment1 = new CreateCommentDto()
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Супер пупер"
            };
            var comment2 = new CreateCommentDto()
            {
                UserId = Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Класс"
            };

            SetDataTest(dataBaseName);

            var result1 = await service.AddCommentAsync(comment1);
            var result2 = await service.AddCommentAsync(comment2);

            //Act
            await service.DeleteCommentAsync(result1, comment1.UserId);

            //Assert + Act

            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.DeleteCommentAsync(result2, comment1.UserId));

        }
        [Fact]
        public async Task UpdateCommentAsync_Test()
        {
            //Arrange
            string dataBaseName = "CommentServiceDBUp";

            var options = new DbContextOptionsBuilder<AdsDbContext>()
               .UseInMemoryDatabase(dataBaseName)
               .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<CommentProfile>();
            configExpression.AddProfile<PagedResultProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            ICommentStorage storage = new CommentStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, storage);
            ICommentEventsDispatcher dispatcher = new CommentEventsDispatcher(new List<ICommentEstimationChangedHandler>());
            ICommentService service = new CommentService(storage, mapper, accessValidations, dispatcher);

            var comment1 = new CreateCommentDto()
            {
                UserId = Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Супер пупер"
            };
            var comment2 = new CreateCommentDto()
            {
                UserId = Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"),
                AdvertisementId = Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                Estimation = 4,
                Text = "Класс"
            };

            await SetDataTest(dataBaseName);

            var result1 = await service.AddCommentAsync(comment1);
            var result2 = await service.AddCommentAsync(comment2);

            var commentUp1 = new UpdateCommentDto()
            {
                Id = result1,
                Estimation = 5,
                Text = "Супер пупер"
            };
            var commentUp2 = new UpdateCommentDto()
            {
                Id = result2,
                Estimation = 2,
                Text = "Разочаровались"
            };

            //Act
            await service.UpdateCommentAsync(commentUp1, Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"));
            await service.UpdateCommentAsync(commentUp2, Guid.Parse("e8b7a2c5-3d14-4f9e-9c6a-2b1d5e7f0a34"));

            var dbComment1 = await storage.GetAsync(result1);
            var dbComment2 = await storage.GetAsync(result2);

            //Assert + Act
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.UpdateCommentAsync(commentUp1, comment2.UserId));

            //Assert
            Assert.Equal(comment1.Text, dbComment1.Text);
            Assert.Equal(comment1.UserId, dbComment1.UserId);
            Assert.Equal(comment1.AdvertisementId, dbComment1.AdvertisementId);
            Assert.NotEqual(comment2.Text, dbComment2.Text);
            Assert.NotEqual(comment2.Estimation, dbComment2.Estimation);

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
