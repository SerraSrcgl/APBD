using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using tutorial6.Middlewares;
using tutorial6.Services;

namespace tutorial6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)  //managing injected services
        {
            services.AddTransient<IStudentsDbService, SqlServerStudentDbService>();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Student API ", Version = "v1" });

            });      //registering swagger generator (swagger documentation)
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env ,IStudentsDbService service)  //specifying our middlewares (icindekilerin hepsi middle ware)
        {                                                                                                           //order onemli  cunku birinden birine gecmesi belli bir sirayla olmali
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //documentation   added to middle wares
            app.UseSwagger();
            app.UseSwaggerUI(c=> 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
            
            }); //creating swagger ui automatically

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LoggingMiddleware>(); //registered our custom middleware

            //inline middleware
            app.Use(async (context ,next) => { 

                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await  context.Response.WriteAsync("index is required"); //message  (async varsa await de olmali)

                    return; //short circutting  not passing to the next middleware direk response yolluyoruz ilerlemeden
                }

                string index = context.Request.Headers["Index"].ToString();
                var student = service.GetStudentbyIndex(index);
                if (student == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Incorrect Index!!"); 
                    return;
                }
                await next(); //calls next middle ware

            });



            //api/v1/students->old version
            //api/v2/students-> new 
            //keeping both endpoints active 
            app.UseRouting();  // this middleware trying to find the appropriate end point but its not executing method yet
             
           // app.UseAuthorization(); dont have permissions for this resource

            app.UseEndpoints(endpoints =>  //responsible for creating new instance then calling the method
            {
                endpoints.MapControllers();
            });
        }
    }
}
