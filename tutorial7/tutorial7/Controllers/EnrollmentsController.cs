using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using tutorial7.DTOs;
using tutorial7.Handlers;
using tutorial7.Models;
using tutorial7.Services;

namespace tutorial7.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    //[Authorize] every end points under this controller will have authorization
    public class EnrollmentsController : ControllerBase
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s19687;Integrated Security=True";

        IStudentsDbService service;
        public IConfiguration Configuration { get; set; }
        //constructor injection

        public EnrollmentsController(IStudentsDbService ser, IConfiguration configuration)
        {
            service = ser;
            Configuration = configuration;
        }

        
     
        [HttpGet]  
        [Authorize(Roles ="admin")] // basicauth icin
        public IActionResult getsmt()
        {
            //User.Claims.ToList(); user bilgilerini alabilirz
            return Ok("hello ");
        }
        
        

        [HttpPost]
        [Authorize(Roles = "employee")]
        //[AllowAnonymous]  -> eger classi komple authorize yaptiysak ve bu metodun authorization olmadan calismasini istiyosak kullanabilirz
        public IActionResult EnrollStudent(Student student)
        {
            Student s = service.EnrollStudent(student);

            return Ok(s);

        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent(Enrollment enrollment)
        {

            Enrollment en = service.PromoteStudent(enrollment);
            return Ok(en);

        }

        [HttpPost("login")]  //tokeni alip diger metodlari o token ile calistiriyoruz.
        public IActionResult Login(LoginRequest request)  //it will generate new token
        {

            bool validation = service.validationCredential(request.Login,request.Password);


            if (validation == false)
            {
                return Unauthorized("incorrect username or password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, request.Login),
                new Claim(ClaimTypes.Name,"name1" ),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                 new Claim(ClaimTypes.Role, "employee")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            //signing token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",  //who issues this token
                audience: "Students",  //who can access it 
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds //which key we will used to encode this token
            );

            //var accesstoken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid();   //can be used when actual token is expired 
                                                 //saved on the client side and db

            service.assignRefreshToken(request.Login, refreshToken);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken 
                                                                 
            });


        }

        [HttpPost("refresh-token")]
        public IActionResult refreshToken(TokenRequest requestToken)
        {
            bool refresh = service.checkrefreshToken(requestToken.refreshToken);


            if (refresh == false)
            {
                return Unauthorized("incorrect refresh token!");
            }

            var claims = new[]
           {
                //new Claim(ClaimTypes.NameIdentifier, request.Login),
                new Claim(ClaimTypes.Name,"name1" ),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                new Claim(ClaimTypes.Role, "employee")

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",  
                audience: "Students", 
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds 
            );



            var newrefreshToken = Guid.NewGuid();                                             
            service.updateRefreshToken(requestToken.refreshToken, newrefreshToken);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                newrefreshToken

            });
        }



    }
}