using Microsoft.AspNetCore.Identity;
using Wookiee.Service.Model.User;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Interface;

public interface IUserService
{
    Task<ResponseObject<string>> Register(RegisterDto user);
    Task<ResponseObject<string>> Login(LoginDto login);
}