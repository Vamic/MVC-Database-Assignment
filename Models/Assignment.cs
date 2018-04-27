using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcDatabase.Models
{
    public class Assignment : DbObject
    {
        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public List<Student> Students { get; set; }
        
        [MaxLength(1024)]
        public string Description { get; set; }

        public virtual Course Course { get; set; }
    }
}