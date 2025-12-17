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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdsManagement.Tests.CommentTests
{
    public class CommentStorageTests
    {
        [Fact]
        public async Task AddAndGetAsync_Test()
        {
            //Arrange
            string nameDB = "CommentDbAG";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDB)
                .Options;

            using var context = new AdsDbContext(options);

            var storage = new CommentStorage(context);

            var comments = new List<Comment>()
            {
                new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                userId: Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Все отлично!!!!", 5),
                new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),userId: Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                "Быстро решили проблему с безопасностью, хакеры не могу прорваться. Рекомендую", 5),

                new Comment(advertisementId: Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), userId: Guid.Parse("5d9f3e2a-7c6b-4a18-8e1f-b2c0d4a9e756"),
                "Компания Информационные сети благодарит IT-компанию Дексмобайл за высокий профессионализм в работе по разработке автоматизированной информационной системы " +
                "GorodPay: интерфейсной части (мобильные приложения) и серверной части системы. Выражаем уверенность в сохранении сложившихся партнерских отношений и надеемся на дальнейшее сотрудничество.", 5),
                new Comment(advertisementId: Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), userId: Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                "Сотрудничество с командой DEX стало приятным и\r\nпродуктивным опытом для нашей компании. Желаем\r\nреализации поставленных задач и развития!", 5),
                new Comment(advertisementId: Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), userId: Guid.Parse("a6d2c4f1-1e59-4b92-8f7e-6c3a9d0e5b21"),
                "Выражаем огромную благодарность команде Dex за\r\nотличную работу над приложением АПК РМ.\r\nКачественно проведенная аналитика, дизайн с акцентом на\r\nюзабилити и стиль, а также профессиональный подход к\r\nразработке и тестированию продукта позволили\r\nразработать удобное мобильное приложение для наших\r\nсельхозтоваропроизводителей.\r\nС удовольствием продолжаем тесное сотрудничество в этом\r\nгоду!",5)
            };

            var commentDop = new Comment(advertisementId: Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), userId: Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"),
                "Невероятно", 5);
            var commentExc = new Comment(advertisementId: Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), userId: Guid.Parse("a6d2c4f1-1e59-4b92-8f7e-6c3a9d0e5b21"),
                "Супер комментарий", 5);
            await SetDataTest(nameDB);

            //Act 

            foreach (var comment in comments)
            {
                await storage.AddAsync(comment);
            }
            Guid dbId = await storage.AddAsync(commentDop);

            var result1 = await storage.GetByAdvertisementAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), 0, -10);
            var resultDEX = await storage.GetByAdvertisementAsync(Guid.Parse("e9a1d7c4-5b2f-4e8a-a6c3-0f9b2d1e7c54"), 1, 10);
            var result_0 = await storage.GetByAdvertisementAsync(Guid.Parse("b2d4f6a1-8c3e-4b9a-92d1-6e5f7a0c4b83"), 1, 10);

            //Assert + Act
            await Assert.ThrowsAsync<CommentAlreadyExistsException>(async () => await storage.AddAsync(commentExc));

            //Assert
            Assert.NotEqual(Guid.Empty, dbId);
            Assert.Equal(2, result1.TotalCount);
            Assert.Equal(4, resultDEX.TotalCount);
            Assert.Equal(0, result_0.TotalCount);
        }
        [Fact]
        public async Task DeleteAsync_Test()
        {
            //Arrange
            string nameDB = "CommentDbDel";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDB)
                .Options;

            using var context = new AdsDbContext(options);

            var storage = new CommentStorage(context);

            var comments = new List<Comment>()
            {
                new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),
                userId: Guid.Parse("9f1a4e1d-2c7a-4b8a-9d6a-1f6b8e4d2a91"), "Все отлично!!!!", 5),
                new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"),userId: Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                "Быстро решили проблему с безопасностью, хакеры не могу прорваться. Рекомендую", 5)
            };
            await SetDataTest(nameDB);


            await storage.AddAsync(comments[0]);
            Guid commentId = await storage.AddAsync(comments[1]);

            //Act
            var resultBefore = await storage.GetByAdvertisementAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), 1, 10);
            await storage.DeleteAsync(commentId);
            var resultAlter = await storage.GetByAdvertisementAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), 1, 10);

            //Assert
            Assert.NotEqual(resultBefore.TotalCount, resultAlter.TotalCount);
            Assert.Equal(1, resultAlter.TotalCount);
        }
        [Fact]
        public async Task UpdateAsync_Test()
        {
            //Arrange
            string nameDB = "CommentDbUpd";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase(nameDB)
                .Options;

            using var context = new AdsDbContext(options);

            var storage = new CommentStorage(context);

            string originalText = "Быстро решили проблему с безопасностью, хакеры не могу прорваться. Рекомендую";
            int originalEstimation = 5;
            var comment = new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), userId: Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                originalText, originalEstimation); 

            await SetDataTest(nameDB);

            Guid commentId = await storage.AddAsync(comment);

            var newComment = new Comment(advertisementId: Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), userId: Guid.Parse("3c7e9b5a-8f24-4c3e-9a61-0b7d2e5f4a88"),
                "Наша компания благодарна.", 4, commentId); 

            //Act
            await storage.UpdateAsync(newComment);
            var dbComment = await storage.GetByAdvertisementAsync(Guid.Parse("7f3a9c8b-2e4d-4c5a-9f6e-1b8e4a0c2d91"), 1, 1);

            //Assert
            Assert.Equal(newComment.Text, dbComment.Items[0].Text);
            Assert.Equal(newComment.Estimation, dbComment.Items[0].Estimation);
            Assert.NotEqual(originalEstimation, dbComment.Items[0].Estimation);
            Assert.NotEqual(originalText, dbComment.Items[0].Text); 
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
            var adStorage = new AdvertisementStorage(dbContext, date, 10);


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
