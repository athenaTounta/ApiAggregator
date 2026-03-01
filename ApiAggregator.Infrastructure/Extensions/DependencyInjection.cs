
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Infrastructure.ExternalClients;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAggregator.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            services.AddHttpClient("ApiAggregator", client =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ApiAggregator");
            });
            services.AddScoped<IExternalApiService, WeatherService>();
            services.AddScoped<IExternalApiService, NewsService>();
            services.AddScoped<IExternalApiService, GitHubService>();
            return services;
        }
    }
}
