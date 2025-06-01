using System.ComponentModel.DataAnnotations;

namespace StickyNotes.Dtos
{
    public class StickyNoteCreateDto
    {
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot be longer than 500 characters.")]
        public string Content { get; set; }
    }

    public class StickyNoteResponseDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot be longer than 500 characters.")]
        public string Content { get; set; }
    }

}
