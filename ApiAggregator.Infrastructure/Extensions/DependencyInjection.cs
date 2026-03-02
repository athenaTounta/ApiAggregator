
using ApiAggregator.Application.Abstractions;
using ApiAggregator.Infrastructure.ExternalClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace ApiAggregator.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        /// <summary>
        /// left the client with the default resilience policy
        /// 3 attempts with exponential backoff (2s, 4s, 8s) + jitter
        /// 30s for entire request including retries
        /// 10s timeout for each attempt
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            services.AddHttpClient("ApiAggregator", client =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("ApiAggregator");
            })
            .AddStandardResilienceHandler();

            services.AddScoped<IExternalApiService, WeatherService>();
            services.AddScoped<IExternalApiService, NewsService>();
            services.AddScoped<IExternalApiService, GitHubService>();
            return services;
        }
    }
}
