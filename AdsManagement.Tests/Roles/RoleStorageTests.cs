using AdsManagement.App.Exceptions;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdsManagement.Tests.Roles
{
    public class RoleStorageTests
    {
        [Fact]
        public async Task GetAndAddAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbGet1")
                .Options;

            using var context = new AdsDbContext(options);
            var rolesStorage = new RoleStorage(context);
            Guid testId = Guid.NewGuid();

            var roles = new List<Role>()
            {
                new Role("SuperAdmin", testId),
                new Role("Admin"),
                new Role("Moderator"),
                new Role("User")
            };
            var role2 = new Role("SuperAdmin");

            //Act
            foreach (var role in roles)
                await rolesStorage.AddAsync(role);

            var dbRole = await rolesStorage.GetAsync(testId);
            var RolePage = await rolesStorage.GetAllAsync(1, 10);

            //Act + Assert
            await Assert.ThrowsAsync<RoleExistsException>(async () =>
            {
                await rolesStorage.AddAsync(role2);
            });

            //Assert
            Assert.NotNull(dbRole);
            Assert.Equal("SuperAdmin", dbRole.Name);
            Assert.Equal(roles.Count, RolePage.TotalCount);
        }
        [Fact]
        public async Task UpdateAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbUp1")
                .Options;

            using var context = new AdsDbContext(options);
            var rolesStorage = new RoleStorage(context);
            Guid testId = Guid.NewGuid();

            var roles = new List<Role>()
            {
                new Role("SuperAdmin", testId),
                new Role("Admin"),
                new Role("Moderator"),
                new Role("User")
            };

            foreach (var role in roles)
                await rolesStorage.AddAsync(role);

            Role upRole = new Role("SuperPuper Admin", testId);

            //Act
            await rolesStorage.UpdateAsync(upRole);
            var dbRole = await rolesStorage.GetAsync(testId);

            //Assert
            Assert.Equal(upRole.Name, dbRole.Name);
        }
        [Fact]
        public async Task DeleteAsync_Test()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("DbDel1")
                .Options;

            using var context = new AdsDbContext(options);
            var rolesStorage = new RoleStorage(context);
            Guid testId = Guid.NewGuid();

            var roles = new List<Role>()
            {
                new Role("SuperAdmin", testId),
                new Role("Admin"),
                new Role("Moderator"),
                new Role("User")
            };

            foreach (var role in roles)
                await rolesStorage.AddAsync(role);

            //Act
            await rolesStorage.DeleteAsync(testId);

            //Act + Assert
            await Assert.ThrowsAsync<RoleNotFoundException>(async () => await rolesStorage.DeleteAsync(testId));
            await Assert.ThrowsAsync<RoleNotFoundException>(async () => await rolesStorage.GetAsync(testId));
        }
    }
}
