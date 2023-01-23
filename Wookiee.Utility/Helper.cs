using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Wookiee.Utility;

public interface IHelper
{
    string? GetLoggedId();
    string CreateToken(string? userId, string? userName);
    string? ImageToBase64(IFormFile? image);
    Dictionary<bool, string?> ImageValidation(IFormFile? image);
}

public class Helper : IHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public Helper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string? GetLoggedId()
    {
        var result = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        return result?.Value;
    }

    public string CreateToken(string? userId, string? userName)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId!),
            new (ClaimTypes.Name, userName!)
        };

        var appSettingToken = _configuration.GetSection("AppSettings:Token").Value;
        if (appSettingToken is null) throw new Exception("AppSettings Token in null");

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingToken));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string? ImageToBase64(IFormFile? image)
    {
        if (image == null) return null;
        var length = image.Length;
        if (length < 0)
            return null;

        using var ms = new MemoryStream();
        image.CopyTo(ms);
        var fileBytes = ms.ToArray();
        return fileBytes.Length > 0 ? GetImageAsBase64(fileBytes) : null;
    }

    public Dictionary<bool, string?> ImageValidation(IFormFile? image)
    {
        if (image == null) return new Dictionary<bool, string?> { { true, null } };

        if (image?.Length >= 200000) return new Dictionary<bool, string?> {{false, "File is too big, limit is 200kb"}};

        var imageExtension = Path.GetExtension(image!.FileName);
        var acceptedFileExtenstion = new List<string> {".jpg", ".jpeg", ".png", ".gif"};
        if (!acceptedFileExtenstion.Contains(imageExtension.ToLower()))
            return new Dictionary<bool, string?> {{false, "Not valid file extenstion"}};
        return new Dictionary<bool, string?>{ {true, null } };
    }

    private string? GetImageAsBase64(byte[] image)
    {
        return Convert.ToBase64String(image);
    }
}