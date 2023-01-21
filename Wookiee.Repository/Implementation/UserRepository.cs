using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wookiee.Model.Entities;
using Wookiee.Repository.Context;
using Wookiee.Repository.Interface;

namespace Wookiee.Repository.Implementation;

public class UserRepository: IUserRepository
{

    #region field

    private readonly WookieeContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    #endregion

    #region constructor

    public UserRepository(WookieeContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #endregion

    #region MyRegion

    public async Task<IdentityResult> Register(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityUser?> FindByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> FindById(string id)
    {
        return (User)(await _userManager.FindByIdAsync(id))!;
    }

    public async Task<List<string>?> FindByAuthorName(string authorPseudonym)
    {
        return await _context.User.Where(u => EF.Functions.Like(u.AuthorPseudonym, $"%{authorPseudonym}%")).Select(x => x.Id).ToListAsync();
    }

    public async Task<SignInResult> Login(string userName, string password, bool rememberMe)
    {
        return await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);
    }

    #endregion

}