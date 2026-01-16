using AdsManagement.API.ApiValidators;
using AdsManagement.API.Middleware;
using AdsManagement.App.Handlers;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Events;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.App.Mappings;
using AdsManagement.App.Services;
using AdsManagement.App.Services.Events;
using AdsManagement.App.Settings;
using AdsManagement.App.Validators.Advertisement;
using AdsManagement.App.Validators.Role;
using AdsManagement.App.Validators.User;
using AdsManagement.Data;
using AdsManagement.Data.Storages;
using AdsManagement.Data.System;
using AdsManagement.FileStorage;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
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

            builder.Services.AddScoped<IUserStorage, UserStorage>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IAdvertisementStorage, AdvertisementStorage>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();

            builder.Services.AddScoped<IFileStorageService, FileStorageService>();

            builder.Services.AddScoped<IAdImageStorage, AdImageStorage>();
            builder.Services.AddScoped<IAdImageService, AdImageService>();
            builder.Services.AddScoped<IImageProcessorService, ImageProcessorService>();

            builder.Services.AddScoped<ICommentStorage, CommentStorage>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            builder.Services.AddScoped<IAccessValidationsService, AccessValidationsService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleDtoValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserFilterDtoValidator>();

            builder.Services.AddValidatorsFromAssemblyContaining<AdFilterDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateAdvertisementDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateAdvertisementDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserAdvertisementFilterDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateAdvertisementRequestValidator>();

            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

            var configExpression = new MapperConfigurationExpression();
            configExpression.AddProfile<RoleProfile>();
            configExpression.AddProfile<PagedResultProfile>();
            configExpression.AddProfile<UserProfile>();
            configExpression.AddProfile<AdvertisementProfile>();
            configExpression.AddProfile<AdImageProfile>();
            configExpression.AddProfile<CommentProfile>();

            var config = new MapperConfiguration(configExpression, new NullLoggerFactory());
            IMapper mapper = config.CreateMapper();

            builder.Services.AddSingleton(mapper);

            builder.Services.Configure<AdServiceSettings>(
                builder.Configuration.GetSection("AdServiceSettings"));

            builder.Services.Configure<AdImageServiceSettings>(
                builder.Configuration.GetSection("AdImageServiceSettings"));

            builder.Services.AddScoped<ICommentEstimationChangedHandler,RecalculateRatingHandler>();
            builder.Services.AddScoped<ICommentEventsDispatcher, CommentEventsDispatcher>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AdsManagement API",
                Version = "v1"
            }));


            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();

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
