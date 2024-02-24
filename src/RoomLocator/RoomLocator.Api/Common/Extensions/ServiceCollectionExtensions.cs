using RoomLocator.Business.Schedules.Interfaces;
using RoomLocator.Business.Schedules.Services;
using RoomLocator.Persistence.Cache;
using StackExchange.Redis;

namespace RoomLocator.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        return services.AddApi().AddBusiness().AddPersistence(configuration);
    }

    private static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<HttpClient>();
        services.AddScoped<IcalService>();
        services.AddScoped<RoomLocatorService>();
        services.AddScoped<KseScheduleProvider>();
        services.AddScoped<HierarchicalRoomsService>();
        services.AddScoped<IIcalService, IcalServiceAdapter>();
        services.AddScoped<IKseScheduleClient, KseScheduleClient>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var redisConfigOptions = new ConfigurationOptions
        {
            Password = configuration["Redis:Password"],
            EndPoints =
            {
                configuration["Redis:EndPoint"],
            },
        };

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfigOptions));
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
