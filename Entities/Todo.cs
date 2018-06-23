using System.ComponentModel.DataAnnotations;
using Entities.Identity;

namespace Entities
{
    public class Todo
    {
        [Key]
        public int TodoId { get; set; }

        [Required]
        public string Description { get; set; }

        // Relationship
        [Required]
        public WebAppUser User { get; set; }
    }
}

