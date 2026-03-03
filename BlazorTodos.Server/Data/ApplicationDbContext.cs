using BlazorTodos.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorTodos.Server.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoTask> TodoTasks { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.HasMany(e => e.Tasks)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId);
            });
            // Configure TodoTask entity
            modelBuilder.Entity<TodoTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description);
                entity.Property(e => e.IsCompleted).IsRequired();
                entity.Property(e => e.Priority).IsRequired();
                entity.Property(e => e.DueDate);
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).IsRequired();
            });
        }

    }
}
