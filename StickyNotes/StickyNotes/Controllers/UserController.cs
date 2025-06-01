using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using StickyNotes.Dtos;
using StickyNotes.Mapping;
using StickyNotes.Models;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UserController : ControllerBase { 
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IUserMapper _usersMapper;

    public UserController(AppDbContext context,IPasswordHasher<User> passwordHasher,IConfiguration configuration,IUserMapper usersMapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _usersMapper = usersMapper;
    }
    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();

        var userResponses = users.Select(n => _usersMapper.MapUserToUserResponse(n)).ToList();

        return Ok(userResponses);
    }

    [Authorize(Roles = "Admin, Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto model)
    {
        if (_context.Users.Any(u => u.Username == model.Username))
        {
            return BadRequest("Username is already taken");
        }


        var user = new User
        {
            Username = model.Username
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User creaeted successfully");
    }

    [Authorize(Roles = "Admin, Manager")]
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) 
            return NotFound();

        user.IsDisabled = dto.IsDisabled;
        //TODO user.IsAdmin = dto.IsAdmin;

        await _context.SaveChangesAsync();

        return Ok("User updated successfully");
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
