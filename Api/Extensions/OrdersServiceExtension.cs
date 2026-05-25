using Api.Services;

namespace Api.Extension
{
    public static class OrdersServiceExtension
    {
        public static IServiceCollection AddOrdersService(
            this IServiceCollection services
        )
        {
            return services.AddScoped<OrdersService>();
        }
    }
}