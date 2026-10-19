using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace FSP_Images.Services;

public class JwtTokenService(IConfiguration config, IWebHostEnvironment env)
{
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly TokenValidationParameters _parameters = BuildParameters(config, env);

    public ClaimsPrincipal? Validate(string accessToken)
    {
        try
        {
            return _handler.ValidateToken(accessToken, _parameters, out _);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return null;
        }
    }

    public bool IsAdmin(ClaimsPrincipal principal) =>
       principal.Claims.Any(c =>
           (c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
           && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase));

    private static TokenValidationParameters BuildParameters(IConfiguration config, IWebHostEnvironment env)
    {
        var pem = File.ReadAllText(Path.Combine(env.ContentRootPath, config["jwt:PublicKeyPath"]!));

        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        var issuer = config["Auth:Issuer"];
        var audience = config["Auth:Audience"];

        return new TokenValidationParameters
        {
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
            ValidateIssuer = !string.IsNullOrEmpty(issuer),
            ValidIssuer = issuer,
            ValidateAudience = !string.IsNullOrEmpty(audience),
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
}
