using System.ComponentModel.DataAnnotations;

namespace StickyNotes.Models
{
    public class StickyNote
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = null!;
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
