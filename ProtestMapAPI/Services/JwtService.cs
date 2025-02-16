using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProtestMapAPI.Data;
using ProtestMapAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ProtestMapAPI.Services;

public class JwtService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public JwtService(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

  public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        return null;

    var userAccount = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
    if (userAccount == null)
        return null;

    var issuer = _configuration["JwtSettings:Issuer"];
    var audience = _configuration["JwtSettings:Audience"];
    var key = _configuration["JwtSettings:Key"];
    var tokenValidityMins = _configuration.GetValue<int>("JwtSettings:TokenValidityMins");
    var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
    var tokenHandler = new JwtSecurityTokenHandler();
    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Name, request.Email),
            new Claim(JwtRegisteredClaimNames.NameId, userAccount.Id) // Add the user ID claim
        }),
        Expires = tokenExpiryTimeStamp,
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
    var accessToken = tokenHandler.WriteToken(securityToken);

    return new LoginResponseModel
    {
        Email = request.Email,
        AccessToken = accessToken,
        ExpiresIn = tokenValidityMins
    };
}
}
