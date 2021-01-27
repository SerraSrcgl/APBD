using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tutorial6.Services;

namespace tutorial6.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;  //readonly ->only assinging on constructor or field
        public LoggingMiddleware(RequestDelegate next) { 
            _next = next; 
        }
        public async Task InvokeAsync(HttpContext context ,IStudentsDbService service)   // middlewarei invoke ettigimz metod
        {
            if(context.Request != null)
            {
                string method = context.Request.Method;
                string path = context.Request.Path.ToString(); //  /api/enrollment
                string queryst = context.Request?.QueryString.ToString();  //nullable
                string body = "";

                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true)) //size of the buffer , leave open(true)
                {
                    body = await reader.ReadToEndAsync();
                }

                var logfile = @"C:\Users\aysen\Desktop\apbd\tutorials\tut6\tutorial6\tutorial6\requestsLog.txt";

               StreamWriter writer = File.AppendText(logfile);

                    writer.WriteLine(method);
                    writer.WriteLine(path);
                    writer.WriteLine(body);
                    writer.WriteLine(queryst);
                    writer.WriteLine("------------------------");
                    writer.Close();

                //or log to database
                service.SaveLogData("data...");

            }
           
            if (_next != null) //if it isnt the last middleware
            {
                await _next(context);  //executes next middleware(passing req)
            }
        }
    }
}
