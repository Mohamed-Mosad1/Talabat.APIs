using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Error;

namespace Talabat.APIs.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IWebHostEnvironment _env;

		public ExceptionMiddleware(
			RequestDelegate next,
			ILogger<ExceptionMiddleware> logger,
			IWebHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				// Take An Action With the request
				await _next.Invoke(httpContext);
				// Take An Action With the response
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message); // Development
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				httpContext.Response.ContentType = "application/json";

				var response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

				if (_env.IsDevelopment())
				{
					response.Message = ex.Message;
					response.Details = ex.StackTrace;
				}

				var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
				var json = JsonSerializer.Serialize(response, options);
				await httpContext.Response.WriteAsync(json);
			}
		}
	}
}
