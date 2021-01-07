using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cw7.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate next;
        private const string LogFileName = "requestsLog.txt";

        public LoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                var path = httpContext.Request.Path; 
                var querystring = httpContext.Request?.QueryString.ToString();
                var method = httpContext.Request.Method;

                using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true);
                var bodyStr = await reader.ReadToEndAsync();
                var logData = $"\n=========\nPath: = {path}\nQuery string: = {querystring}\nMathod: = {method}\nBody: = {bodyStr}";
                var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                
                await File.AppendAllTextAsync(Path.Combine(currentDomainBaseDirectory, LogFileName), logData);
            }

            await next(httpContext);
        }
    }
}
