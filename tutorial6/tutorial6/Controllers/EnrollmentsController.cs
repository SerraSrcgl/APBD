using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tutorial6.Models;
using tutorial6.Services;

namespace tutorial6.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s19687;Integrated Security=True";
        IStudentsDbService service;

        //constructor injection

        public EnrollmentsController(IStudentsDbService ser)
        {
            service = ser;
        }


        [HttpPost]
        public IActionResult EnrollStudent(Student student)
        {
            Student s = service.EnrollStudent(student);

            return Ok(s);

        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudent(Enrollment enrollment)
        {

            Enrollment en = service.PromoteStudent(enrollment);
            return Ok(en);

        }

    }
}