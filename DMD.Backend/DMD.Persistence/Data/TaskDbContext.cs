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
                .HasMany(t => t.SubTasks) // Указывает, что одна задача может иметь множество подзадач
                .WithOne() // Не имеет родительской задачи, это будет настроено через внешний ключ
                .HasForeignKey(t => t.ParentTaskID) // Указывает, что ParentTaskID является внешним ключом
                .OnDelete(DeleteBehavior.Cascade); // Удаление задачи удаляет её подзадачи

            // Ограничение на статус задачи
            modelBuilder.Entity<TodoTask>()
                .Property(t => t.Status)
                .HasConversion<string>() // Преобразование статуса в строку
                .HasMaxLength(50) // Ограничение длины строки
                .HasDefaultValue("Назначена") // Значение по умолчанию
                .IsRequired(); // Статус обязателен

            // Добавление ограничения CHECK для статусов
            modelBuilder.Entity<TodoTask>()
                .ToTable(t => t.HasCheckConstraint("CK_Task_Status", "Status IN ('Назначена', 'Выполняется', 'Приостановлена', 'Завершена')"));

            modelBuilder.Entity<TodoTask>()
                .Property(t => t.RegistrationDate)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();
        }
    }

}