using Microsoft.Extensions.DependencyInjection;

namespace XClaim.Web.Components.Extensions;

public static class ComponentServerExtensions {
    public static IServiceCollection RegisterServerRazorExtensions(this IServiceCollection services) {
        services.AddScoped<Navigation>();

        return services;
    }

    public static IServiceCollection RegisterServerBlazorState(this IServiceCollection services) {

        return services;
    }
}