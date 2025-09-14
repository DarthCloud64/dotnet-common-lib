using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Catch invoked in ExceptionHandlingMiddleware");

            ErrorResponseDto errorResponseDto = new ErrorResponseDto
            {
                Message = exception.Message,
            };

            switch (exception)
            {
                case BadRequestException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponseDto));
                    break;
                case NotFoundException e:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponseDto));
                    break;
                case Exception e:
                    errorResponseDto = new ErrorResponseDto
                    {
                        Message = "Unexpected error occurred.",
                    };
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponseDto));
                    break;
            }
        }
    }
}