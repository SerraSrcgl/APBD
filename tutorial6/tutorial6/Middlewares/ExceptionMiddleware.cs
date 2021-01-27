using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutorial6.Models;

namespace tutorial6.Middlewares
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;  //readonly ->only assinging on constructor or field
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)   // middlewarei invoke ettigimz metod
        {

            try
            {
                await _next(context);
            }catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails { 
                StatusCode= StatusCodes.Status500InternalServerError,
              
                Message="Error Happened!"
            }.ToString());
        }
    }
}
