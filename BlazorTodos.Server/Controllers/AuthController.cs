using Microsoft.AspNetCore.Mvc;
using BlazorTodos.Server.Data;
using BlazorTodos.Server.Models;
using BlazorTodos.Server.Helpers;
using BlazorTodos.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorTodos.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var hashedPassword = PasswordHasher.HashPassword(request.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == hashedPassword);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Role = user.Role
            });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, CreateTodoDto dto)
        {
            if (dto.Priority == null)
                return BadRequest("Priority is required.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null)
                return NotFound();

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.Priority = dto.Priority.Value;
            todo.DueDate = dto.DueDate;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}