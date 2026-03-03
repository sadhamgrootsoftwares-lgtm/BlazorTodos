using System.ComponentModel.DataAnnotations;

namespace BlazorTodos.Server.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // Admin / User

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<TodoTask> Tasks { get; set; }

    }
}
