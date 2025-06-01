using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StickyNotes.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;
        public bool IsDisabled { get; set; } = false;
        public ICollection<StickyNote> StickyNotes { get; set; } = new List<StickyNote>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
