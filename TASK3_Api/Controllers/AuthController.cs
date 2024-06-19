using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TASK3_Business.Dtos.AuthDtos;
using TASK3_Business.Services.Interfaces;
using TASK3_Core.Entities;

namespace TASK3_Api.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase {
    private readonly IAuthService _authService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) {
      _authService = authService;
      _roleManager = roleManager;
      _userManager = userManager;
    }

    [HttpGet("create-users")]
    [Authorize(Roles = "Developer")]
    public async Task<IActionResult> CreateUser() {

      await _roleManager.CreateAsync(new IdentityRole("Admin"));
      await _roleManager.CreateAsync(new IdentityRole("Member"));


      AppUser user1 = new() {
        FullName = "Admin",
        UserName = "admin",
      };

      await _userManager.CreateAsync(user1, "Admin123");

      AppUser user2 = new() {
        FullName = "Member",
        UserName = "member",
      };

      await _userManager.CreateAsync(user2, "Member123");

      await _userManager.AddToRoleAsync(user1, "Admin");
      await _userManager.AddToRoleAsync(user2, "Member");

      return Ok("Users created Created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto) {
      var token = await _authService.Login(loginDto);
      return Ok(new { token });
    }
  }
}
