using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    [Entity]
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string DepartmentId { get; set; }
        public int Credits { get; set; }
    }
}
