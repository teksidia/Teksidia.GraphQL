using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    [Entity]
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Forename { get; set; }
        public int CourseId { get; set; }

        [Child("CourseId", "CourseId")]
        public Course Course { get; set; }
    }
}
