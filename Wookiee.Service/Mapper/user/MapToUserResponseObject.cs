using Wookiee.Utility.Response;

namespace Wookiee.Service.Mapper.user;

public class MapToUserResponseObject
{
    public static ResponseObject<string> ToResponseObject(string? result, bool isSuccess, object? errorMessage, Exception? exception)
    {
        return new ResponseObject<string>
        {
            Exception = exception,
            ErrorMessage = errorMessage,
            IsSuccess = isSuccess,
            Result = result
        };
    }
}