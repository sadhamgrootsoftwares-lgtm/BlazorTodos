
using BlazorTodos.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorTodos.Server.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityLevel Priority { get; set; }
        public DateTime? DueDate { get; set; }

        // Foreign key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}