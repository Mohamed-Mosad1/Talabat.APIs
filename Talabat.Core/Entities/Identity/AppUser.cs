﻿using Microsoft.AspNetCore.Identity;

namespace Talabat.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;

        public UserAddress? Address { get; set; }
    }
}