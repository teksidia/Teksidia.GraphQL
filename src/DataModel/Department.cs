using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    [Entity]
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}
