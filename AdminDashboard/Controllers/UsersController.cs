using AdminDashboard.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Error;
using Talabat.Core.Entities.Identity;

namespace AdminDashboard.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var mappedUsers = users.Select(U => new UserViewModel()
            {
                Id = U.Id,
                UserName = U.UserName ?? string.Empty,
                DisplayName = U.DisplayName,
                Email = U.Email ?? string.Empty,
                PhoneNumber = U.PhoneNumber ?? string.Empty,
                Roles = _userManager.GetRolesAsync(U).Result
            }).ToList();

            return View(mappedUsers);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new ApiResponse(404));

            var allRoles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UserRoleViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Roles = allRoles.Select(R => new RoleViewModel()
                {
                    Id = R.Id,
                    Name = R.Name ?? string.Empty,
                    IsSelected = _userManager.IsInRoleAsync(user, R.Name ?? string.Empty).Result
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);

            if (user == null) return NotFound(viewModel);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in viewModel.Roles)
            {
                if (role.IsSelected && !userRoles.Contains(role.Name))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!role.IsSelected && userRoles.Contains(role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction(nameof(Index));
        }






    }
}
