using Backend.Common.Exception;
using Backend.Contract.Entity;

namespace my_project_backend.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next,ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }catch(Exception ex)
            {
                await OnExceptionAsync(ex, context);
            }
        }

        private async Task OnExceptionAsync(Exception exception,HttpContext context)
        {
            logger.LogError(exception,exception.Message);
            if(exception is InvalidOperationException or AddAccountFailureException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(
                    CustomResponse<object>.Failure(StatusCodes.Status400BadRequest, exception.Message));
            }else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    CustomResponse<object>.Failure(StatusCodes.Status500InternalServerError, "服务器出错了！"));
            }
        }
    }
}
