using AdsManagement.App.Validators.Role;
using AdsManagement.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Data.Storages;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Services;
using AutoMapper;
using AdsManagement.App.Mappings;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging.Abstractions;
namespace AdsManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AdsDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRoleStorage, RoleStorage>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleDtoValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            var config = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = config.CreateMapper();

            builder.Services.AddSingleton(mapper);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AdsManagement API",
                Version = "v1"
            }));


            var app = builder.Build();
            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "AdsManagement API V1");
                });
            }

            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}
