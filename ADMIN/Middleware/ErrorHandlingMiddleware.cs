namespace ADMIN.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Tiếp tục xử lý request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Bắt lỗi và xử lý tại đây
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // Thiết lập mã trạng thái HTTP
            context.Response.StatusCode = statusCode;

            // Render trang HTML tương ứng
            string errorPagePath = statusCode switch
            {
                (int)HttpStatusCode.Unauthorized => "/Error/Unauthorized",
                (int)HttpStatusCode.NotFound => "/Error/NotFound",
                _ => "/Error/InternalServerError"
            };

            // Chuyển hướng đến trang lỗi
            context.Response.Redirect(errorPagePath);

            return Task.CompletedTask;
        }
    }

}
