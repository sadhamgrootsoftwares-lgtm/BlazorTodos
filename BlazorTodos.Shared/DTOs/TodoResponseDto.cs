using BlazorTodos.Shared.Enums;

namespace BlazorTodos.Shared.DTOs
{
    public class TodoResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}