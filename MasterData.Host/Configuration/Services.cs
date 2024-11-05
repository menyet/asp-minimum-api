using MasterData.Domain;
using MasterData.Domain.Cache;
using MasterData.Host.Endpoints;

namespace MasterData.Host.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        var assembly = typeof(VendorOperations).Assembly;
        var types = assembly.GetTypes().Where(_ => _.IsClass && !_.IsAbstract).ToArray();

        foreach (var t in types.Where(_ => _.IsAssignableTo(typeof(IDbFacade))))
        {
            foreach (var i in t.GetInterfaces().Where(_ => _.Assembly == assembly))
            {
                services.AddScoped(i, t);
            }
        }

        services.AddScoped<IItemCache, ItemCache>();

        return services;
    }
}
