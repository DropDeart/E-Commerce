using Ljuss.Core.Configurations;
using Ljuss.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using LjussAPI.Application.DTOs.MiddlewareDTOs;

namespace LjussAPI.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (CustomException ex)
            {
                _logger.LogError(ex, "An error occurred..");
                var errorResponse = new ErrorResponseDto(
                    ex.Message,
                    ex.Message,
                    ex.ErrorCodes
                );
                httpContext.Response.StatusCode = (int)ex.ErrorCodes;
                httpContext.Response.ContentType = "application/json";
                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                var errorResponse = new ErrorResponseDto(
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    ErrorCodesEnum.InternalServerError
                );
                httpContext.Response.StatusCode = 500;
                httpContext.Response.ContentType = "application/json";
                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
