using Microsoft.AspNetCore.Identity;
using Wookiee.Model.Entities;

namespace Wookiee.Repository.Interface;

public interface IUserRepository
{
    Task<IdentityResult> Register(User user, string password);
    Task<IdentityUser?> FindByEmail(string email);
    Task<SignInResult> Login(string userName, string password, bool rememberMe);
    Task<User?> FindById(string id);
    Task<List<string>?> FindByAuthorName(string authorPseudonym);
}