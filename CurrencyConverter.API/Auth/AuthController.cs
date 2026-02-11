using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim("ClientId", Guid.NewGuid().ToString())
    };

        var jwtSettings = _configuration.GetSection("Jwt");
        var key = jwtSettings["Key"];

        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

        var credentials =
            new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

}
