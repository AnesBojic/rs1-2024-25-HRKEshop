namespace RS1_2024_25.API.Helper
{


    public static class ApiErrorCodes
    {
        public const string InvalidCredentials = "InvalidCredentials";
        public const string AccountLocked = "AccountLocked";
        public const string EmailNotVerified = "EmailNotVerified";
    }


    public class ApiErrorResponse
    {

        public string ErrorCode { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public ApiErrorResponse(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

    }
}
