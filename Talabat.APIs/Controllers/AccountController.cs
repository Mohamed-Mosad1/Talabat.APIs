using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.APIs.Extentions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;

        public AccountController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            ITokenServices tokenServices,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _mapper = mapper;
        }
        // Register
        [HttpPost("register")] // POST : /api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }

        // Login
        [HttpPost("login")] // POST : /api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) return Unauthorized(new ApiResponse(401, "Login Failed"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return Unauthorized(new ApiResponse(401, "Login Failed"));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet] // GET : /api/account
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto()
            {
                DisplayName = user?.DisplayName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Token = await _tokenServices.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet("userAddress")] // GET : /api/account/userAddress
        public async Task<ActionResult<UserAddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);

            if (user == null || user.Address == null)
            {
                return NotFound(new ApiResponse(404, "user address not found."));
            }

            var userAddressDto = _mapper.Map<UserAddressDto>(user.Address);
            return Ok(userAddressDto);
        }

        [Authorize]
        [HttpPut("userAddress")] // PUT : /api/account/userAddress
        public async Task<ActionResult<UserAddressDto>> UpdateUserAddress(UserAddressDto userAddress)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);

            if (user == null || user.Address == null)
            {
                return NotFound(new ApiResponse(404, "user address not found."));
            }

            var updateAddress = _mapper.Map<UserAddressDto, UserAddress>(userAddress);
            updateAddress.Id = user.Address.Id;
            user.Address = updateAddress;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });
            }

            // Map updated address back to DTO for response
            var updatedAddressDto = _mapper.Map<UserAddressDto>(updateAddress);
            return Ok(updatedAddressDto);
        }



    }
}
