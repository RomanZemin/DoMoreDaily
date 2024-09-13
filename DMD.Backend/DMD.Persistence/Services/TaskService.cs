using AutoMapper;
using DMD.Application.DTOs;
using DMD.Application.Interfaces;
using DMD.Domain.Entities;
using DMD.Persistence.Data;
using Microsoft.EntityFrameworkCore;

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
            var allTasks = await _context.Tasks
                .AsNoTracking()
                .ToListAsync();

            TaskDto MapTask(TodoTask task, List<TodoTask> allTasks)
            {
                var taskDto = _mapper.Map<TaskDto>(task);

                var subTasks = allTasks.Where(t => t.ParentTaskID == task.Id).ToList();

                if (subTasks.Any())
                {
                    taskDto.SubTasks = subTasks.Select(subTask => MapTask(subTask, allTasks)).ToList();
                }

                return taskDto;
            }

            var rootTasks = allTasks
                .Where(t => t.ParentTaskID == null)
                .OrderBy(t => t.TaskName)
                .ToList();

            var result = rootTasks.Select(task => MapTask(task, allTasks)).ToList();

            return result;
        }



        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);
            return _mapper.Map<TaskDto>(task);
        }

        public async Task CreateTaskAsync(TodoTask task)
        {
            task.RegistrationDate = task.RegistrationDate.ToUniversalTime();
            if (task.SubTasks != null && task.SubTasks.Any())
            {
                foreach (var subTask in task.SubTasks)
                {
                    subTask.ParentTaskID = task.Id;
                }
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TodoTask task)
        {
            if (task.RegistrationDate.Kind == DateTimeKind.Unspecified)
            {
                task.RegistrationDate = DateTime.SpecifyKind(task.RegistrationDate, DateTimeKind.Local).ToUniversalTime();
            }
            else if (task.RegistrationDate.Kind == DateTimeKind.Local)
            {
                task.RegistrationDate = task.RegistrationDate.ToUniversalTime();
            }

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        private async Task<bool> HasSubTasksAsync(int taskId)
        {
            return await _context.Tasks.AnyAsync(t => t.ParentTaskID == taskId);
        }
        public async Task ChangeTaskStatusAsync(int taskId, string status)
        {
            var task = await _context.Tasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

            if (!IsValidStatus(status))
                throw new InvalidOperationException("Недопустимый статус задачи.");

            if (status == "Завершена")
            {
                if (task.Status != "Выполняется")
                    throw new InvalidOperationException("Задачу можно завершить только после статуса 'Выполняется'.");

                if (task.SubTasks.Any(st => st.Status != "Завершена"))
                    throw new InvalidOperationException("Нельзя завершить задачу, если не завершены все подзадачи.");

                task.Status = "Завершена";
                foreach (var subtask in task.SubTasks)
                {
                    subtask.Status = "Завершена";
                }
            }
            else if (status == "Приостановлена")
            {
                if (task.Status != "Выполняется")
                    throw new InvalidOperationException("Статус 'Приостановлена' можно установить только после статуса 'Выполняется'.");
                task.Status = "Приостановлена";
            }
            else
            {
                task.Status = status;
            }

            await _context.SaveChangesAsync();
        }

        private bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "Назначена", "Выполняется", "Приостановлена", "Завершена" };
            return validStatuses.Contains(status);
        }
    }
}
