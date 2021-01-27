using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tutorial5.Models
{
    public class Student
    {
        [Required]  //if user not provide index then it will return 400 Bad request
        [RegularExpression("^s[0-9]+$")]
        [MaxLength(100)]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string studies { get; set; }   //Computer Science

    }
}
