using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StickyNotes.Dtos;
using StickyNotes.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Data;
using StickyNotes.Services;
using Microsoft.AspNetCore.Http;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration, ILogger<AuthController> logger, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Register new user with password and username
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/register
    ///     {
    ///         "username": "johndoe",
    ///         "password": "StrongPassword123"
    ///     }
    /// </remarks>
    /// <param name="model">Object containing the username and password.</param>
    /// /// <returns>Returns a success message or an error if the username is already taken.</returns>
    /// <returns></returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Username is already taken</response>
    /// <response code="500">Unexpected server error</response>
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto model)
    {
        _logger.LogInformation($"Registering user: username {model.Username}, password {model.Password}");


        if (_context.Users.Any(u => u.Username == model.Username))
        {
            _logger.LogInformation($"Registering user: username taken");
            return BadRequest("Username is already taken");
        }

        var user = new User
        {
            Username = model.Username
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"User registered: username {model.Username}, password {model.Password}, userID: {user.Id}");


        return Ok("User registered successfully");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserCreateDto model)
    {
        _logger.LogInformation($"Login attempt: username {model.Username}, password {model.Password}");

        var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.Username == model.Username);
        if (user == null)
        {
            _logger.LogInformation($"Login failed: Invalid username or password, username {model.Username}, password {model.Password}");
            return Unauthorized("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogInformation($"Login failed: Invalid username or password, username {model.Username}, password {model.Password}");
            return Unauthorized("Invalid username or password");
        }

        var token = _tokenService.GenerateJwtToken(user);

        _logger.LogInformation($"Login successful: username {model.Username}");

        return Ok(new { Token = token });
    }
}
