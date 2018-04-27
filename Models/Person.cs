using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcDatabase.Models
{
    abstract public class Person : DbObject
    {        
        public virtual List<Course> Courses { get; set; }
    }
}