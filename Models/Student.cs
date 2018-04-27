using System.Collections.Generic;

namespace MvcDatabase.Models
{
    public class Student : Person
    {
        public virtual List<Assignment> Assignments { get; set; }
    }
}