using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tutorial5.Models;

namespace tutorial5.Services
{
    public interface IStudentsDbService
    {

        Student EnrollStudent(Student student);
        Enrollment PromoteStudent(Enrollment enrollment);
    }
}
