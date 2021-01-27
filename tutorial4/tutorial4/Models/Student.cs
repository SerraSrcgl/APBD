using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tutorial4.Models
{
    public class Student
    {
        [Required(ErrorMessage ="you should enter a name!")] //checking the validation kullanici null yada bos biraktiysa vs diye
        [MaxLength(20)]  //post calismadan once apicontoller validationi check etcek hata varsa o method calismicak
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string studies { get; set; }
        public int semester { get; set; }
    }
}
