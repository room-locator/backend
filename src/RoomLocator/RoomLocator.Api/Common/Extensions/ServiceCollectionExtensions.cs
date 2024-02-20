using RoomLocator.Business.Schedules.Interfaces;
using RoomLocator.Business.Schedules.Services;

namespace RoomLocator.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        return services.AddApi().AddBusiness();
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
        services.AddSingleton<HttpClient>();
        services.AddSingleton<IcalService>();
        services.AddSingleton<ScheduleService>();
        services.AddSingleton<KseScheduleProvider>();
        services.AddSingleton<IIcalService, IcalServiceAdapter>();
        services.AddSingleton<IKseScheduleClient, KseScheduleClient>();

        return services;
    }
}
