using DMD.Persistence.Data;
using DMD.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using DMD.Domain.Entities;

namespace DMD.Persistence.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;

        public TaskService(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
        {
            return await _context.Tasks.Include(t => t.SubTasks).ToListAsync();
        }

        public async Task<TodoTask> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTaskAsync(TodoTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TodoTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || task.SubTasks.Any())
                throw new InvalidOperationException("Невозможно удалить задачу с подзадачами или несуществующую задачу.");
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeTaskStatusAsync(int taskId, string status)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

            if (status == "Завершена")
            {
                if (task.SubTasks.Any(st => st.Status != "Завершена"))
                    throw new InvalidOperationException("Нельзя завершить задачу, если не завершены все подзадачи.");
                task.Status = status;
                foreach (var subtask in task.SubTasks)
                {
                    subtask.Status = "Завершена";
                }
            }
            else
            {
                task.Status = status;
            }
            await _context.SaveChangesAsync();
        }
    }

}
