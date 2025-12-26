using AdsManagement.App.DTOs.User;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Mappings;
using AdsManagement.App.Services;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdsManagement.Tests.Users
{
    public class UserServiceTests
    {
        private Guid _idRoleUser = Guid.NewGuid();
        private Guid _idRoleAdmin = Guid.NewGuid();
        [Fact]
        public async Task AddUserAsync_And_GetUserAsync_Test()
        {
            //Arrange 
            string dataBaseName = "UserServiceDbAdd";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
           .UseInMemoryDatabase(dataBaseName)
           .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<UserProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<RoleProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            IUserStorage storage = new UserStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, default);
            IUserService service = new UserService(storage, mapper, accessValidations);

            CreateUserDto userDto = new CreateUserDto() { Name = "Test 1", RoleId = _idRoleUser };
            CreateUserDto userDto2 = new CreateUserDto() { Name = "Test 2", RoleId = Guid.NewGuid() };

            await SetDataTest(dataBaseName);
            //Act
            var result1 = await service.AddUserAsync(userDto);
            var userDb = await service.GetUserAsync(result1);

            //Assert + Act
            await Assert.ThrowsAsync<RoleNotFoundException>(async () => await service.AddUserAsync(userDto2));
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await service.GetUserAsync(Guid.NewGuid()));

            //Assert
            Assert.NotEqual(Guid.Empty, result1);
            Assert.NotNull(userDb);

        }
        [Fact]
        public async Task DeleteUserAsync_Test()
        {
            //Arrange 
            string dataBaseName = "UserServiceDbADel";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
           .UseInMemoryDatabase(dataBaseName)
           .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<UserProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<RoleProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            IUserStorage storage = new UserStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, default);
            IUserService service = new UserService(storage, mapper, accessValidations);

            CreateUserDto userDto = new CreateUserDto() { Name = "Test 1", RoleId = _idRoleUser };
            CreateUserDto userDto2 = new CreateUserDto() { Name = "Test 2", RoleId = _idRoleUser };

            await SetDataTest(dataBaseName);
            var id1 = await service.AddUserAsync(userDto);
            var id2 = await service.AddUserAsync(userDto2);

            //Act
            await service.DeleteUserAsync(id1, id1);

            //Assert + Act
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.DeleteUserAsync(id2, id1));
        }
        [Fact]
        public async Task UpdateUserAsync_Test()
        {
            //Arrange 
            string dataBaseName = "UserServiceDbUp";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
           .UseInMemoryDatabase(dataBaseName)
           .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<UserProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<RoleProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();

            using var dbContext = new AdsDbContext(options);
            IUserStorage storage = new UserStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, default);
            IUserService service = new UserService(storage, mapper, accessValidations);

            CreateUserDto userDto = new CreateUserDto() { Name = "Test 1", RoleId = _idRoleUser };
            CreateUserDto userDto2 = new CreateUserDto() { Name = "Test 2", RoleId = _idRoleUser };

            await SetDataTest(dataBaseName);
            var id1 = await service.AddUserAsync(userDto);
            var id2 = await service.AddUserAsync(userDto2);

            UpdateUserDto up1 = new UpdateUserDto() { Id = id1, Name = "Test 11", RoleId = _idRoleUser };
            UpdateUserDto up2 = new UpdateUserDto() { Id = id2, RoleId = _idRoleAdmin };

            //Act
            await service.UpdateUserAsync(up1, id1);
            await service.UpdateUserAsync(up2, id2);

            var dbUser1 = await service.GetUserAsync(id1);
            var dbUser2 = await service.GetUserAsync(id2);

            //Act + Assert
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.UpdateUserAsync(up1, id2));
            await Assert.ThrowsAsync<AccessDeniedException>(async () => await service.UpdateUserAsync(up2, id1));

            //Assert
            Assert.Equal(up1.Name, dbUser1.Name);
            Assert.Equal(up1.RoleId, dbUser1.Role.Id);
            Assert.NotEqual(userDto.Name, dbUser1.Name);

            Assert.Equal(up2.RoleId, dbUser2.Role.Id);
            Assert.Equal(up2.Name, dbUser2.Name);
            Assert.NotEqual(userDto2.RoleId, dbUser2.Role.Id);

        }
        [Fact]
        public async Task GetFilterUserAsync_Test()
        {
            //Arrange 
            string dataBaseName = "UserServiceDbFilter";
            var options = new DbContextOptionsBuilder<AdsDbContext>()
           .UseInMemoryDatabase(dataBaseName)
           .Options;

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<UserProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<RoleProfile>();

            var mapConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapConfig.CreateMapper();


            using var dbContext = new AdsDbContext(options);
            IUserStorage storage = new UserStorage(dbContext);
            IAccessValidationsService accessValidations = new AccessValidationsService(default, default);
            IUserService service = new UserService(storage, mapper, accessValidations);

            List<CreateUserDto> dtos = new List<CreateUserDto>()
            {
                new CreateUserDto() { Name = "Test 1", RoleId = _idRoleUser },
                new CreateUserDto() { Name = "Test 2", RoleId = _idRoleUser },
                new CreateUserDto() { Name = "Test 3", RoleId = _idRoleUser },
                new CreateUserDto() { Name = "Test 4", RoleId = _idRoleAdmin },
                new CreateUserDto() { Name = "Test 5", RoleId = _idRoleAdmin },
            };

            await SetDataTest(dataBaseName);
            foreach (var user in dtos)
                await service.AddUserAsync(user);

            UserFilterDto filter1_User = new UserFilterDto()
            {
                Name = "Test",
                RoleId = _idRoleUser,
                SortBy = "Name"
            };
            UserFilterDto filter1_Admin = new UserFilterDto()
            {
                Name = "Test",
                RoleId = _idRoleAdmin,
                SortBy = "Name",
                SortDesc = true
            };

            //Act
            var result1 = await service.GetFilterUserAsync(filter1_User);
            var result2 = await service.GetFilterUserAsync(filter1_Admin);

            //Assert
            Assert.Equal(3, result1.TotalCount);
            Assert.Equal(2, result2.TotalCount);
            Assert.Equal(dtos.Last().Name, result2.Items.First().Name);
            Assert.Equal(dtos.First().Name, result1.Items.First().Name);
        }
        private async Task SetDataTest(string dataBaseName)
        {
            var options = new DbContextOptionsBuilder<AdsDbContext>()
             .UseInMemoryDatabase(dataBaseName)
             .Options;

            using var dbContext = new AdsDbContext(options);
            var roleStorage = new RoleStorage(dbContext);

            Role role = new Role("User", _idRoleUser);
            Role roleAdm = new Role("Admin", _idRoleAdmin);

            await roleStorage.AddAsync(role);
            await roleStorage.AddAsync(roleAdm);
        }
    }
}
