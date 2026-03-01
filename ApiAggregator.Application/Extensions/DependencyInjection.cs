using ApiAggregator.Application.Abstractions;
using ApiAggregator.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAggregator.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services)
        {
            services.AddScoped<IAggregationService, AggregationService>();
            return services;
        }
    }
}
