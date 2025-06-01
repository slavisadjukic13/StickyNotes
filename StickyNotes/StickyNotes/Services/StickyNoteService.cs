using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StickyNotes.Dtos;
using StickyNotes.Mapping;
using StickyNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StickyNotes.Services
{

    public interface IStickyNoteService
    {
        Task<IEnumerable<StickyNoteResponseDto>> GetStickyNotesForUserAsync(int userId, int page, int pageSize, string query);
        Task<StickyNoteResponseDto?> GetStickyNoteByIdAsync(int id, int userId);
        Task<StickyNoteResponseDto> CreateStickyNoteAsync(StickyNoteCreateDto dto, int userId);
        Task<bool> UpdateStickyNoteAsync(int id, StickyNoteCreateDto dto, int userId);
        Task<bool> DeleteStickyNoteAsync(int id, int userId);
    }

    public class StickyNoteService : IStickyNoteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StickyNoteService> _logger;
        private readonly IStickyNoteMapper _mapper;

        public StickyNoteService(AppDbContext context, ILogger<StickyNoteService> logger, IStickyNoteMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;   
        }

        public async Task<IEnumerable<StickyNoteResponseDto>> GetStickyNotesForUserAsync(int userId, int page, int pageSize, string query)
        {
            _logger.LogInformation("Fetching sticky notes for user {UserId}", userId);

            var queryable = _context.StickyNotes.Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(n => EF.Functions.Like(n.Content, $"%{query}%"));
            }

            var stickyNotes = await queryable
                .OrderByDescending(n => n.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return stickyNotes.Select(_mapper.MapStickyNoteToStickyNoteResponseDto);
        }

        public async Task<StickyNoteResponseDto?> GetStickyNoteByIdAsync(int id, int userId)
        {
            _logger.LogInformation("Fetching sticky note {NoteId} for user {UserId}", id, userId);

            var stickyNote =  await _context.StickyNotes
                .Where(n => n.Id == id && n.UserId == userId).FirstOrDefaultAsync();

            return _mapper.MapStickyNoteToStickyNoteResponseDto(stickyNote);


        }

        public async Task<StickyNoteResponseDto> CreateStickyNoteAsync(StickyNoteCreateDto dto, int userId)
        {

            StickyNote stickyNote = _mapper.MapStickyNoteCreateToStickyNote(userId, dto);

            _context.StickyNotes.Add(stickyNote);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Sticky note {note.Id} created for user {UserId}", stickyNote.Id, userId);


            return _mapper.MapStickyNoteToStickyNoteResponseDto(stickyNote);
        }

        public async Task<bool> UpdateStickyNoteAsync(int id, StickyNoteCreateDto dto, int userId)
        {
            var note = await _context.StickyNotes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (note == null) {
                _logger.LogWarning("Sticky note {NoteId} not found for user {UserId}", id, userId);
                return false;
            } 

            note.Content = dto.Content;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Sticky note {NoteId} updated for user {UserId}", id, userId);

            return true;
        }

        public async Task<bool> DeleteStickyNoteAsync(int id, int userId)
        {
            var note = await _context.StickyNotes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (note == null) return false;

            _context.StickyNotes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
