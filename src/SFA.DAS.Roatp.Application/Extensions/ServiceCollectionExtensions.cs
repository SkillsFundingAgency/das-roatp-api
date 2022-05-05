using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Roatp.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationRegistrations(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceCollectionExtensions));
        }
    }
}