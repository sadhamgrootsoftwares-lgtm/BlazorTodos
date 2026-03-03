using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorTodos.Server.Data;
using BlazorTodos.Server.Models;
using BlazorTodos.Shared.DTOs;
using System.Security.Claims;

namespace BlazorTodos.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET USER TASKS
        [HttpGet]
        public async Task<IActionResult> GetUserTodos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todos = await _context.TodoTasks
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .Select(t => new TodoResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority,
                    DueDate = t.DueDate
                })
                .ToListAsync();

            return Ok(todos);
        }

        // CREATE TODO
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoDto dto)
        {
            if (dto.Priority == null)
                return BadRequest("Priority is required.");
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = new TodoTask
            {
                Title = dto.Title,
                Description = dto.Description,
                //Priority = dto.Priority ?? BlazorTodos.Shared.Enums.PriorityLevel.Medium,
                Priority = dto.Priority.Value,
                DueDate = dto.DueDate,
                UserId = userId
            };

            _context.TodoTasks.Add(todo);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE TODO
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null)
                return NotFound();

            todo.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // TOGGLE COMPLETE
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null)
                return NotFound();

            todo.IsCompleted = !todo.IsCompleted;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
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
            todo.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }

}
