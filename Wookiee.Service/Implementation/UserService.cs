using Microsoft.Extensions.Logging;
using Wookiee.Model.Entities;
using Wookiee.Repository.Interface;
using Wookiee.Service.Interface;
using Wookiee.Service.Model.User;
using Wookiee.Utility;
using Wookiee.Utility.Response;

namespace Wookiee.Service.Implementation;

public class UserService: IUserService
{

    #region field

    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IHelper _helper;

    #endregion

    #region constructor

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, IHelper helper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _helper = helper;
    }

    #endregion

    #region implementation

    public async Task<ResponseObject<string>> Register(RegisterDto user)
    {
        try
        {
            var identityResult = await _userRepository.Register(new User
            {
                UserName = user.UserName,
                Email = user.Email,
                AuthorPseudonym = user.AuthorPseudonym,
                FirstName = user.FirstName,
                LastName = user.LastName,
            },
                user.Password!);

            return identityResult.Succeeded 
                ? Mapper.user.MapToUserResponseObject.ToResponseObject("User is created", identityResult.Succeeded,
                    null, null)
                : Mapper.user.MapToUserResponseObject.ToResponseObject(null, identityResult.Succeeded,
                    identityResult.Errors.Select(x => x.Description), null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Register user going wrong");
            return Mapper.user.MapToUserResponseObject.ToResponseObject(null, false,
                e.Message, e);
        }
    }

    public async Task<ResponseObject<string>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userRepository.FindByEmail(loginDto.Email!);
            if (user == null)
                return Mapper.user.MapToUserResponseObject.ToResponseObject(null, false,
                    "Login failed, please check email or password", null);

            var signInResult = await _userRepository.Login(user.UserName!, loginDto.Password!, loginDto.RememberMe);
            if (!signInResult.Succeeded)
                return Mapper.user.MapToUserResponseObject.ToResponseObject(null, false,
                "Login failed, please check email or password", null);

            if (!string.IsNullOrWhiteSpace(user.Id))
                return Mapper.user.MapToUserResponseObject.ToResponseObject(_helper.CreateToken(user.Id, user.UserName!), true,
                    null, null);

            return Mapper.user.MapToUserResponseObject.ToResponseObject(null, false,
                "Login failed, please check email or password", null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "login going wrong");
            return Mapper.user.MapToUserResponseObject.ToResponseObject(null, false,
                e.Message, e);
        }

    }

    #endregion
}