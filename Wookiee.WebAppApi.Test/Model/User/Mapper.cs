using Wookiee.Service.Model.User;
using Wookiee.Utility.Response;
using Wookiee.WebAppApi.PostData.User;

namespace Wookiee.WebAppApi.Test.Model.User;

public static class Mapper
{
    public static Register ToRegisterUser()
    {
        return new Register
        {
            AuthorPseudonym = "Test author",
            Email = "Test",
            FirstName = "Test first name",
            LastName = "Test last name",
            Password = "testPassword",
        };
    }

    public static Login ToLoginUser()
    {
        return new Login
        {
            Email = "Test@test.nl",
            Password = "testPassword",
            RememberMe = true,
        };
    }

    public static ResponseObject<string> ToResponseObject()
    {
        return new ResponseObject<string>
        {
            Exception = null,
            ErrorMessage = null,
            IsSuccess = true,
            Result = "Done",
        };
    }
}