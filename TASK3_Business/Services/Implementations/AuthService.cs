using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TASK3_Business.Dtos.AuthDtos;
using TASK3_Business.Exceptions;
using TASK3_Business.Services.Interfaces;
using TASK3_Core.Entities;

namespace TASK3_Business.Services.Implementations {
  public class AuthService : IAuthService {
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<AppUser> userManager, IConfiguration configuration) {
      _userManager = userManager;
      _configuration = configuration;
    }
    public async Task<string> Login(LoginDto loginDto) {
      AppUser? user = await _userManager.FindByNameAsync(loginDto.UserName);

      if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");

      List<Claim> claims = new();
      claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
      claims.Add(new Claim(ClaimTypes.Name, user.UserName));
      claims.Add(new Claim("FullName", user.FullName));

      var roles = await _userManager.GetRolesAsync(user);

      claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

      string secret = _configuration.GetSection("JWT:Secret").Value!;

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

      JwtSecurityToken securityToken = new(
          issuer: _configuration.GetSection("JWT:Issuer").Value,
          audience: _configuration.GetSection("JWT:Audience").Value,
          claims: claims,
          signingCredentials: creds,
          expires: DateTime.Now.AddDays(3)
          );

      string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

      return token;
    }
  }
}
