using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TASK3_UI.Resources;

namespace TASK3_UI.Filters {
  public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
      try {
        await _next(context);
      }
      catch (HttpResponseException ex) {
        _logger.LogError($"HttpResponseException caught: {ex.Message}");
        await HandleHttpResponseExceptionAsync(context, ex);
      }
      catch (Exception ex) {
        _logger.LogError($"Unhandled exception caught: {ex.Message}");
        await HandleExceptionAsync(context, ex);
      }
    }

    private static Task HandleHttpResponseExceptionAsync(HttpContext context, HttpResponseException ex) {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)ex.Response.StatusCode;

      ErrorResponse errorResponse;
      try {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var responseContent = ex.Response.Content.ReadAsStringAsync().Result;
        errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, options);
      }
      catch (Exception deserializationEx) {
        errorResponse = new ErrorResponse {
          StatusCode = context.Response.StatusCode,
          Message = ex.Response.ReasonPhrase ?? "An error occurred processing your request.",
          Errors = [ new ErrorResponseItem { Key = "DeserializationError", Message = deserializationEx.Message } ]
        };
      }

      errorResponse ??= new ErrorResponse {
        StatusCode = context.Response.StatusCode,
        Message = ex.Response.ReasonPhrase ?? "An error occurred processing your request.",
        Errors = []
      };

      var result = JsonSerializer.Serialize(errorResponse);
      return context.Response.WriteAsync(result);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex) {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

      var errorResponse = new ErrorResponse {
        StatusCode = context.Response.StatusCode,
        Message = "An internal server error has occurred.",
        Errors = [ new ErrorResponseItem { Key = "Exception", Message = ex.Message } ]
      };

      var result = JsonSerializer.Serialize(errorResponse);
      return context.Response.WriteAsync(result);
    }
  }
}
