using System;
using System.Collections.Generic;

namespace tutorial9.Models
{
    public partial class Enrollment
    {
        public Enrollment()
        {
            Student = new HashSet<Student>();
        }
        //[Key] PK
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public int IdStudy { get; set; } //FK
        public DateTime StartDate { get; set; }

        public virtual Studies IdStudyNavigation { get; set; }
        public virtual ICollection<Student> Student { get; set; }
    }
}
