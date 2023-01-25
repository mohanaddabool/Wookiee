using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Wookiee.Utility;

#region interface

public interface IHelper
{
    string? GetLoggedId();
    string CreateToken(string? userId, string? userName);
    string? ImageToBase64(IFormFile? image);
    (bool isSucess, string? errorMessage) ImageValidation(IFormFile? image);
}

#endregion

#region implementation

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

    public (bool isSucess, string? errorMessage) ImageValidation(IFormFile? image)
    {
        if (image == null) return (true, null);

        if (image.Length >= 200000) return (false, "File is too big, limit is 200kb");

        var imageExtension = Path.GetExtension(image.FileName);
        var acceptedFileExtenstion = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
        return !acceptedFileExtenstion.Contains(imageExtension.ToLower())
            ? (false, "Not valid file extenstion")
            : (true, null);
    }

    #endregion

    #region private methods

    private static string GetImageAsBase64(byte[] image)
    {
        return Convert.ToBase64String(image);
    }

    #endregion

}