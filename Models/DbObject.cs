using System.ComponentModel.DataAnnotations;

namespace MvcDatabase.Models
{
    abstract public class DbObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}