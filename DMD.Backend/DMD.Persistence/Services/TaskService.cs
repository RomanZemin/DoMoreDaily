using AutoMapper;
using DMD.Persistence.Data;
using DMD.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using DMD.Domain.Entities;
using DMD.Application.DTOs;
using System.Threading.Tasks;

namespace DMD.Persistence.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(TaskDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _context.Tasks
                .Include(t => t.SubTasks) // Загрузка подзадач
                .ToListAsync();
            return _mapper.Map<IEnumerable<TaskDto>>(tasks);
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);
            return _mapper.Map<TaskDto>(task);
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
            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

            if (await HasSubTasksAsync(id))
                throw new InvalidOperationException("Невозможно удалить задачу с подзадачами.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        private async Task<bool> HasSubTasksAsync(int taskId)
        {
            return await _context.Tasks.AnyAsync(t => t.ParentTaskID == taskId);
        }
        private bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Назначена", "Выполняется", "Приостановлена", "Завершена" };
            return validStatuses.Contains(status);
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
