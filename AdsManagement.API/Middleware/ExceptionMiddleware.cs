using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using System.Net;
using System.Text.Json;

namespace AdsManagement.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            switch(ex)
            {
                case EntityNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case AccessDeniedException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;

                case ExceedingTheAdLimitException:
                case AdvertisementImageLimitExceededException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;

                case CommentAlreadyExistsException:
                case RoleExistsException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;

                case InvalidFileWeightException:
                    context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                    break;

                case Exception:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var response = new
            {
                error = ex.GetType().Name,
                message = ex.Message,
                stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
