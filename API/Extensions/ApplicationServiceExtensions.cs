using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationService(this IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddControllers();
        serviceCollection.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        serviceCollection.AddCors();
        serviceCollection.AddScoped<ITokenService, TokenService>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IPhotoService, PhotoService>();
        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        serviceCollection.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        
        return serviceCollection;
    }
}
