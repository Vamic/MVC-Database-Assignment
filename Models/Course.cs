using System.Collections.Generic;

namespace MvcDatabase.Models
{
    public class Course : DbObject
    {
        public virtual List<Student> Students { get; set; }
        public virtual List<Teacher> Teachers { get; set; }
        public virtual List<Assignment> Assignments { get; set; }
    }
}