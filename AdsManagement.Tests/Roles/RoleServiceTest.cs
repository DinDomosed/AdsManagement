using AdsManagement.App.DTOs.Role;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Mappings;
using AdsManagement.App.Services;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AdsManagement.Tests.Roles
{
    public class RoleServiceTest
    {
        [Fact]
        public async Task AddRoleAsync_Test()
        {
            //Arrange
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();
            

            var mapperConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapperConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("RoleServiceDbAdd")
                .Options;

            var context = new AdsDbContext(options);
            IRoleStorage storage = new RoleStorage(context);
            IRoleService service = new RoleService(storage, mapper);

            var role1 = new CreateRoleDto() { Name = "SuperAdmin"};
            var role2 = new CreateRoleDto() { Name = "Admin"};
            var role3 = new CreateRoleDto() { Name = "User"};

            var role1_2 = new CreateRoleDto() { Name = "SuperAdmin"};

            //Act 
            var result1 = await service.AddRoleAsync(role1);
            var result2 = await service.AddRoleAsync(role2);
            var result3 = await service.AddRoleAsync(role3);

            //Assert + Act

            await Assert.ThrowsAsync<RoleExistsException>(async () => await service.AddRoleAsync(role1_2));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddRoleAsync(null));

            //Assert
            Assert.NotEqual(Guid.Empty, result1);
            Assert.NotEqual(Guid.Empty, result2);
            Assert.NotEqual(Guid.Empty, result3);
        }
        [Fact]
        public async Task GetRoleAsync_Test()
        {
            //Arrange
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();
            configExpression.AddProfile<PagedResultProfile>();

            var mapperConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapperConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("RoleServiceDbGet")
                .Options;

            var context = new AdsDbContext(options);
            IRoleStorage storage = new RoleStorage(context);
            IRoleService service = new RoleService(storage, mapper);

            var role1 = new CreateRoleDto() { Name = "SuperAdmin" };
            var role2 = new CreateRoleDto() { Name = "Admin" };
            var role3 = new CreateRoleDto() { Name = "User" };

            var result1 = await service.AddRoleAsync(role1);
            var result2 = await service.AddRoleAsync(role2);
            var result3 = await service.AddRoleAsync(role3);

            //Act
            var resultRole = await service.GetRoleAsync(result1);
            var resultAll = await service.GetAllRolesAsync();

            //Assert
            Assert.True(resultRole is ResponseRoleDto);
            Assert.Equal(role1.Name, resultRole.Name);
            Assert.Equal(3, resultAll.TotalCount);
        }
        [Fact]
        public async Task DeleteRoleAsync_Test()
        {
            //Arrange
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();

            var mapperConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapperConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("RoleServiceDbDel")
                .Options;

            var context = new AdsDbContext(options);
            IRoleStorage storage = new RoleStorage(context);
            IRoleService service = new RoleService(storage, mapper);

            var role1 = new CreateRoleDto() { Name = "SuperAdmin" };
            var role2 = new CreateRoleDto() { Name = "Admin" };

            var result1ID = await service.AddRoleAsync(role1);
            var result2ID = await service.AddRoleAsync(role2);

            //Act
            await service.DeleteRoleAsync(result1ID);
            var role = await service.GetRoleAsync(result2ID);

            //Assert + Act
            await Assert.ThrowsAsync<RoleNotFoundException>(async () => await service.GetRoleAsync(result1ID));

            //Assert
            Assert.NotNull(role);
        }
        [Fact]
        public async Task UpdateRoleAsync_Test()
        {
            //Arrange
            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();

            var mapperConfig = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = mapperConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<AdsDbContext>()
                .UseInMemoryDatabase("RoleServiceDbUp")
                .Options;

            var context = new AdsDbContext(options);
            IRoleStorage storage = new RoleStorage(context);
            IRoleService service = new RoleService(storage, mapper);

            var role1 = new CreateRoleDto() { Name = "SuperAdmin" };
            var role2 = new CreateRoleDto() { Name = "Admin" };


            var result1ID = await service.AddRoleAsync(role1);
            var result2ID = await service.AddRoleAsync(role2);

            var roleUpDto = new UpdateRoleDto() { Id = result1ID, Name = "SuperPuperAdmin" };

            //Act
            await service.UpdateRoleAsync(roleUpDto);
            var roleDb1 = await service.GetRoleAsync(result1ID);
            var roleDb2 = await service.GetRoleAsync(result2ID);

            //Assert
            Assert.Equal(role2.Name, roleDb2.Name);
            Assert.NotEqual(role1.Name, roleDb1.Name);
            Assert.Equal(roleUpDto.Name, roleDb1.Name);
        }
    }
}
