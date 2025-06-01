using StickyNotes.Models;
using StickyNotes.Dtos;

namespace StickyNotes.Mapping
{
    public interface IStickyNoteMapper
    {
        StickyNote MapStickyNoteCreateToStickyNote(int userId, StickyNoteCreateDto stickyNoteCreateDto);
        StickyNoteResponseDto MapStickyNoteToStickyNoteResponseDto(StickyNote stickyNote);

    }

    public class StickyNoteMapper : IStickyNoteMapper
    {
        public StickyNote MapStickyNoteCreateToStickyNote(int userId, StickyNoteCreateDto stickyNoteCreateDto)
        {
            return new StickyNote
            {
                Content = stickyNoteCreateDto.Content,
                UserId = userId
            };

        }

        public StickyNoteResponseDto MapStickyNoteToStickyNoteResponseDto(StickyNote stickyNote)
        {
            return new StickyNoteResponseDto
            {
                Content = stickyNote.Content,
                Id = stickyNote.Id
            };

        }
    }
}
