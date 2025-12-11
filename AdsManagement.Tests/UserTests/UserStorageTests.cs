using AdsManagement.App.DTOs.User;
using AdsManagement.App.Exceptions;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace AdsManagement.Tests.UserTests
{ 
    public class UserStorageTests
    {
        [Fact]
        public async Task AddAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbAdd0")
                .Options;

            using var dbContext = new AdsDbContext(options);
            var roleStorage = new RoleStorage(dbContext);

            var userStorage = new UserStorage(dbContext);

            Role role = new Role("Admin", Guid.NewGuid());

            List<User> users = new List<User>
            {
                new User("Test0", role.Id, Guid.NewGuid()),
                new User("Test1", role.Id, Guid.NewGuid()),
                new User("Test2", role.Id, Guid.NewGuid()),
                new User("Test3", role.Id, Guid.NewGuid()),
                new User("Test4", role.Id, Guid.NewGuid()),
            };

            var user2 = new User("Test00", Guid.NewGuid());

            await roleStorage.AddAsync(role);

            //Act
            foreach(var entity in users)
            {
                await userStorage.AddAsync(entity);
            }

            var filters = new UserFilterDto()
            {
                Name = "        "
            };
            //Act + Assert
            await Assert.ThrowsAsync<RoleNotFoundException>(async () =>
            {
                await userStorage.AddAsync(user2);
            });

            var dbItems = await userStorage.GetFilterUserAsync(filters);
            //Assert
            Assert.Equal(users.Count, dbItems.TotalCount);
        }
        [Fact]
        public async Task GetAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbGet0")
                .Options;

            using var dbContext = new AdsDbContext(options);
            var roleStorage = new RoleStorage(dbContext);

            var userStorage = new UserStorage(dbContext);

            Role role = new Role("Admin");
            var testId = Guid.NewGuid();

            List<User> users = new List<User>
            {
                new User("Test0", role.Id, Guid.NewGuid()),
                new User("Test1", role.Id, Guid.NewGuid()),
                new User("Test2", role.Id, Guid.NewGuid()),
                new User("Test3", role.Id, testId),
                new User("Test4", role.Id, Guid.NewGuid()),
            };

            await roleStorage.AddAsync(role);

            foreach (var entity in users)
            {
                await userStorage.AddAsync(entity);
            }

            //Act
            var dbUser = await userStorage.GetAsync(testId);

            //Assert
            Assert.NotNull(dbUser);
            Assert.Equal("Test3", dbUser.Name);
        }
        [Fact]
        public async Task DeleteAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbDelete0")
                .Options;

            using var dbContext = new AdsDbContext(options);
            var roleStorage = new RoleStorage(dbContext);
            var userStorage = new UserStorage(dbContext);

            Role role = new Role("Admin");
            var testId = Guid.NewGuid();

            List<User> users = new List<User>
            {
                new User("Test0", role.Id, Guid.NewGuid()),
                new User("Test1", role.Id, Guid.NewGuid()),
                new User("Test2", role.Id, Guid.NewGuid()),
                new User("Test3", role.Id, testId),
                new User("Test4", role.Id, Guid.NewGuid()),
            };

            await roleStorage.AddAsync(role);

            foreach (var entity in users)
            {
                await userStorage.AddAsync(entity);
            }

            //Act
            bool result = await userStorage.DeleteAsync(testId);
            var dbUser = await userStorage.GetAsync(testId);

            //Assert
            Assert.True(result);
            Assert.Null(dbUser);
        }
        [Fact]
        public async Task UpdateAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbUpdate0")
                .Options;

            using var dbContext = new AdsDbContext(options);
            var userStorage = new UserStorage(dbContext);
            var rolesStorage = new RoleStorage(dbContext);

            Role testRole = new Role("Admin");
            Role upRole = new Role("User");
            var testId = Guid.NewGuid();
            var testId1 = Guid.NewGuid(); 

            List<User> users = new List<User>
            {
                new User("Test0", testRole.Id, Guid.NewGuid()),
                new User("Test1", testRole.Id, testId1),
                new User("Test2", testRole.Id, Guid.NewGuid()),
                new User("Test3", testRole.Id, testId),
                new User("Test4", testRole.Id, Guid.NewGuid()),
            };

            await rolesStorage.AddAsync(upRole);
            await rolesStorage.AddAsync(testRole);

            foreach (var user in users)
                await userStorage.AddAsync(user);


            var upUser3 = new User("Test30", upRole.Id, testId);
            var upUser1 = new User("Test11", testRole.Id, testId1);

            //Act

            var result = await userStorage.UpdateAsync(upUser3);
            var result1 = await userStorage.UpdateAsync(upUser1);

            var dbUser = await userStorage.GetAsync(testId);
            var dbUser1 = await userStorage.GetAsync(testId1);

            //Assert
            Assert.NotNull(dbUser.Role);
            Assert.NotNull(dbUser1.Role);

            Assert.True(result);
            Assert.True(result1);

            Assert.Equal("Test30", dbUser.Name);
            Assert.Equal(upRole.Name, dbUser.Role.Name);

            Assert.Equal("Test11", dbUser1.Name);
            Assert.Equal(testRole.Name, dbUser1.Role.Name);

        }
        [Fact]
        public async Task GetFilterUserAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbUpdate01")
                .Options;

            using var dbContext = new AdsDbContext(options);
            var userStorage = new UserStorage(dbContext);
            var rolesStorage = new RoleStorage(dbContext);

            Role testRole = new Role("Admin");
            Role testRole2 = new Role("User");
            var testId = Guid.NewGuid();
            var testId1 = Guid.NewGuid();

            List<User> users = new List<User>
            {
                new User("Test0", testRole.Id, Guid.NewGuid()),
                new User("Test1", testRole2.Id, testId1),
                new User("Test2", testRole.Id, Guid.NewGuid()),
                new User("Test10", testRole2.Id, testId),
                new User("Test100", testRole.Id, Guid.NewGuid()),
            };

            await rolesStorage.AddAsync(testRole);
            await rolesStorage.AddAsync(testRole2);

            foreach (var user in users)
                await userStorage.AddAsync(user);

            var filter = new UserFilterDto()
            {
                Name = "1",
                RoleId = testRole2.Id,
                SortBy = "Role"
            };

            //Act

            var result = await userStorage.GetFilterUserAsync(filter);

            //Assert
            Assert.Equal(2, result.TotalCount);
        }
    }
}
