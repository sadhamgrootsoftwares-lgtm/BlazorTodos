using BlazorTodos.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlazorTodos.Shared.DTOs
{
    public class CreateTodoDto
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        [Required]
        public PriorityLevel? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}