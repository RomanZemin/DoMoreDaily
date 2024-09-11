using Microsoft.EntityFrameworkCore;
using DMD.Domain.Entities;

namespace DMD.Persistence.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<TodoTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTask>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ограничение на статус задачи
            modelBuilder.Entity<TodoTask>()
                .Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue("Назначена")
                .IsRequired();

            // Добавление ограничения CHECK для статусов
            modelBuilder.Entity<TodoTask>()
                .ToTable(t => t.HasCheckConstraint("CK_Task_Status", "[Status] IN ('Назначена', 'Выполняется', 'Приостановлена', 'Завершена')"));
        }
    }

}