using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using StickyNotes.Dtos;
using StickyNotes.Services;
using Microsoft.Extensions.Logging;
using StickyNotes.Mapping;
using StickyNotes.Models;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class StickyNotesController : ControllerBase
{
    private readonly IStickyNoteService _stickyNoteService;
    private readonly ILogger<StickyNotesController> _logger;
    private readonly IStickyNoteMapper _stickyNoteMapper;

    public StickyNotesController(IStickyNoteService stickyNoteService, ILogger<StickyNotesController> logger, IStickyNoteMapper stickyNoteMapper)
    {
        _stickyNoteService = stickyNoteService;
        _logger = logger;
        _stickyNoteMapper = stickyNoteMapper;
    }

    // Helper: Get current user ID from JWT
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    // GET: api/stickynotes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StickyNoteResponseDto>>> GetMyStickyNotes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string query = null )
        
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation("Getting sticky notes for user {UserId} with page {Page}, pageSize {PageSize}, and query '{Query}'", userId, page, pageSize, query);
        var notes = await _stickyNoteService.GetStickyNotesForUserAsync(userId, page, pageSize, query);

        return Ok(notes);
    }

    // GET: api/stickynotes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StickyNoteResponseDto>> GetStickyNote(int id)
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation("User {UserId} requested sticky note {NoteId}", userId, id);

        var note = await _stickyNoteService.GetStickyNoteByIdAsync(id, userId);
        if (note == null) {
            _logger.LogWarning("Sticky note {NoteId} not found for user {UserId}", id, userId);
            return NotFound();
        }
        return Ok(note);
    }

    // POST: api/stickynotes
    [HttpPost]
    public async Task<ActionResult<StickyNoteResponseDto>> CreateStickyNote([FromBody] StickyNoteCreateDto dto)
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation("Creating new sticky note for user {UserId}", userId);

        var createdNote = await _stickyNoteService.CreateStickyNoteAsync(dto, userId);
        _logger.LogInformation("Sticky note {NoteId} created for user {UserId}", createdNote.Id, userId);

        return CreatedAtAction(nameof(GetStickyNote), new { id = createdNote.Id }, createdNote);
    }

    // PUT: api/stickynotes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStickyNote(int id, [FromBody] StickyNoteCreateDto dto)
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation("User {UserId} updating sticky note {NoteId}", userId, id);

        var updated = await _stickyNoteService.UpdateStickyNoteAsync(id, dto, userId);
        if (!updated) 
        {
            _logger.LogWarning("Sticky note {NoteId} not found for user {UserId} during update", id, userId);
            return NotFound();
        }

        _logger.LogInformation("Sticky note {NoteId} updated for user {UserId}", id, userId);
        return NoContent();
    }

    // DELETE: api/stickynotes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStickyNote(int id)
    {
        var userId = GetUserIdFromToken();
        _logger.LogInformation("User {UserId} deleting sticky note {NoteId}", userId, id);

        var deleted = await _stickyNoteService.DeleteStickyNoteAsync(id, userId);
        if (!deleted)  {

            _logger.LogWarning("Sticky note {NoteId} not found for user {UserId} during deletion", id, userId);
            return NotFound();
        }
        _logger.LogInformation("Sticky note {NoteId} deleted for user {UserId}", id, userId);
        return NoContent();
    }
}
