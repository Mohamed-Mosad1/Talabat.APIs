using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Repository.Identity;

namespace Talabat.APIs.Extentions
{
    public static class IdentityServicesExtention
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            //var builder = services.AddIdentityCore<AppUser>();

            //builder = new IdentityBuilder(builder.UserType, builder.Services);

            //builder.AddEntityFrameworkStores<AppIdentityDbContext>();
            //builder.AddSignInManager<AppUser>();

            //services.AddAuthentication();



            return services;
        }
    }
}
