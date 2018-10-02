using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Identity;

namespace Entities
{
    public class Todo
    {
        [Key]
        public int TodoId { get; set; }

        [ForeignKey("WebAppUser")]
        public string UserId { get; set; }

        [Required]
        public string Description { get; set; }

        // Relationship
        public virtual WebAppUser User { get; set; }
    }
}

