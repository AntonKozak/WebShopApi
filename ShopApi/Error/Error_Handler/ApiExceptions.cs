
namespace ShopApi.Error.Error_Handler;

public class ApiExceptions
{
    public ApiExceptions( int statusCode, string message = null, string details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
}
