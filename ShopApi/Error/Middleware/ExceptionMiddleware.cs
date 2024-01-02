
using System.Net;
using System.Text.Json;
using ShopApi.Error.Error_Handler;

namespace ShopApi.Error.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next, // RequestDelegate is a delegate that can handle HTTP requests and responses 
        ILogger<ExceptionMiddleware> logger, // Logger writing out exeption to the console 
        IHostEnvironment env) // IHostEnvironment is an interface that provides information about the web hosting environment Develop or Production mode..
    {
        _env = env;
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // if there is no exception we want to continue with the next middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";  
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // 500

            var response = _env.IsDevelopment()  
                ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) // if we are in development mode we want to see the stack trace
                : new ApiExceptions(context.Response.StatusCode, "Internal Server Error"); // if we are in production mode we don't want to see the stack trace

            var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};// create a new JsonSerializerOptions object

            var json = JsonSerializer.Serialize(response, options);  // serialize the response to json

            await context.Response.WriteAsync(json); // write the json to the response
        }
    }
}
