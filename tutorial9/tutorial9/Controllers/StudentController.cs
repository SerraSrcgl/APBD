using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tutorial9.DTOs;
using tutorial9.Models;

namespace tutorial9.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly s19687Context _dbcontext;  //db

        public StudentController(s19687Context context)
        {
            _dbcontext = context;
        }

        [HttpGet]
        public IActionResult getStudents()
        {
            var res = _dbcontext.Student.Select(e => new { e.IndexNumber, e.FirstName, e.LastName, e.BirthDate, e.IdEnrollment }).ToList();
            return Ok(res);
             
        }

        [HttpPost]
        public IActionResult insertStudent(Student student)
        {
            var res = _dbcontext.Student.Any(e => e.IndexNumber == student.IndexNumber);
            if(res == true)
            {
                return BadRequest("There is a student with this index!");

            }else
            {

                 // _dbcontext.AddRange(); adding list of students
                _dbcontext.Student.Add(student);
                _dbcontext.SaveChanges(); // single transaction  , insert update delete yapinca save yapmak gerekli.
                return Ok("Succesfully added!");
            }
      
        }

        [HttpPut]
        public IActionResult modifyStudent(Student student)
        {
            var res = _dbcontext.Student.Any(e => e.IndexNumber == student.IndexNumber);

            if (res == true)
            {
                var res2 = _dbcontext.Student.Find(student.IndexNumber);
                res2.FirstName = student.FirstName;
                res2.LastName = student.LastName;
                res2.BirthDate=student.BirthDate;
                _dbcontext.SaveChanges();
                return Ok("Succesfully updated!");

            }
            else
            {
                return NotFound("There is no such student");
            }
        }


        [HttpDelete("{index}")]
        public IActionResult deleteStudent(string index)
        {
            var res = _dbcontext.Student.Any(e => e.IndexNumber == index );
            if (res == true)
            {
                
                var res2 = _dbcontext.Student.Find(index);  // get the object by the pk
                _dbcontext.Student.Remove(res2);
                _dbcontext.SaveChanges();
                return Ok("Succesfully deleted!");

                /*
                var student = new Student()
                {
                    IndexNumber = index
                };
                _dbcontext.Attach(student);
                _dbcontext.Entry(student).State = EntityState.Deleted;
                _dbcontext.SaveChanges();
                */

            }
            else
            {
                return NotFound("There is no student with this id!");
            }
        }


        [HttpPost("enrollStudent")]
        public IActionResult EnrollStudent(StudentRequest req)
        {
            var res = _dbcontext.Studies.Any(e => e.Name == req.Studies);

            int idStudy;
            int idEnrollment;
            if (res == true)
            {
                var res2 = _dbcontext.Studies.Where(e => e.Name == req.Studies).Single();
                idStudy = res2.IdStudy;
                var res3 = _dbcontext.Enrollment.Where(e => e.Semester == 1 && e.IdStudy == idStudy).ToList() ;
                
               
                //var res4 = _dbcontext.Enrollment.Any(e => e.StartDate==res3.Max(e=>e.StartDate));
                             
                if (res3.Count>0)
                {
                    var a = res3.Where(e => e.StartDate == res3.Max(e => e.StartDate)).Single();
                    idEnrollment = a.IdEnrollment;

                }
                else
                {
                    DateTime todaysDate = DateTime.Now;
                    var newEnrollment = new Enrollment()
                    {
                        IdEnrollment = _dbcontext.Enrollment.Max(e => e.IdEnrollment) + 1,
                        IdStudy = idStudy,
                        Semester = 1,
                        StartDate = todaysDate
                    };

                    _dbcontext.Enrollment.Add(newEnrollment);
                    _dbcontext.SaveChanges();

                    idEnrollment = newEnrollment.IdEnrollment;
                }

            }
            else
            {
                return NotFound("There is no such studies!");
            }

            var res5 = _dbcontext.Student.Any(e => e.IndexNumber == req.IndexNumber);
            if(res5 == false)
            {
                var newStudent = new Student()
                {
                    IndexNumber = req.IndexNumber,
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    BirthDate = req.BirthDate,
                    IdEnrollment = idEnrollment
                };
                _dbcontext.Student.Add(newStudent);
                _dbcontext.SaveChanges();
                return Ok("succesfully added!");               
            }
            else
            {
                return BadRequest("such student exist in db");
            }
        }

       
        [HttpPost("promoteStudent")]
        public IActionResult PromoteStudent(PromoteRequest promote)
        {
            var res = _dbcontext.Studies.Any(e => e.Name == promote.Studies);
          
            if(res == true)
            {
                var res2 = _dbcontext.Studies.Where(e => e.Name == promote.Studies).Single();
                int idStudy = res2.IdStudy;
                var res3 = _dbcontext.Enrollment.Where(e => e.Semester == promote.Semester && e.IdStudy == idStudy).ToList();
                if (res3.Count > 0)
                {
                    _dbcontext.Database.ExecuteSqlRaw("exec Promote '"+promote.Studies+"',"+promote.Semester+" ;");  
                    _dbcontext.SaveChanges();
                    return Ok("succesfully promoted!");
                }else
                {
                    return NotFound("There is no record with this semester!");
                }
            }
            else
            {
                return NotFound("There is no such studies!");
            }
        }

   
    }
}