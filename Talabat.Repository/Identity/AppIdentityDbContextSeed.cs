using Microsoft.AspNetCore.Identity;

namespace Talabat.Core.Entities.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            if (!_userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Mohamed Mosad",
                    Email = "mohamed@gmail.com",
                    UserName = "mohamed11",
                    PhoneNumber = "01124581819"
                };

                await _userManager.CreateAsync(user, "P@$$W0rd");
            }
        }
    }
}
