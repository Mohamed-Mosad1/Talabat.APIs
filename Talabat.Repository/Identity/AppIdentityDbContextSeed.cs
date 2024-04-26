using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            if (_userManager.Users.Count() == 0)
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
