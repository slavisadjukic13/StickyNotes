using System.ComponentModel.DataAnnotations;

namespace StickyNotes.Dtos
{
    public class UserCreateDto
    {
        /// <summary>
        /// Desired Username for new user
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = null!;
        /// <summary>
        /// Desired password for new user
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = null!;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public bool IsDisabled { get; set; } = false;

    }
    public class UserUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User Id must be greater than zero.")]
        public int Id { get; set; }
        // TODO: public bool IsAdmin { get; set; } = false;
        public bool IsDisabled { get; set; } = false;
    }
}
