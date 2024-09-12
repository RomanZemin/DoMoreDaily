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
                .AsNoTracking()
                .Where(t => t.ParentTaskID == null)
                .Include(t => t.SubTasks)
                .ToListAsync();
            foreach (var task in tasks)
            {
                Console.WriteLine($"Task: {task.TaskName}, SubTasks Count: {task.SubTasks.Count}");
            }

            // Рекурсивный метод для получения всех подзадач
            IEnumerable<TaskDto> GetAllSubTasks(TodoTask task)
            {
                // Маппинг текущей задачи в DTO
                var taskDto = _mapper.Map<TaskDto>(task);

                // Проверка на наличие подзадач и их рекурсивная обработка
                if (task.SubTasks != null && task.SubTasks.Any())
                {
                    // Для каждой подзадачи рекурсивно получаем подподзадачи
                    taskDto.SubTasks = task.SubTasks
                        .Select(subTask => _mapper.Map<TaskDto>(subTask))
                        .ToList();

                    foreach (var subTaskDto in taskDto.SubTasks)
                    {
                        // Найдем подзадачу в оригинальных задачах и добавим её подподзадачи
                        var originalSubTask = task.SubTasks.First(t => t.Id == subTaskDto.Id);
                        subTaskDto.SubTasks = GetAllSubTasks(originalSubTask).ToList();
                    }
                }

                return new List<TaskDto> { taskDto };
            }

            // Обрабатываем все верхнеуровневые задачи и рекурсивно получаем все подзадачи
            var result = tasks
                .SelectMany(t => GetAllSubTasks(t))
                .ToList();

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

            // Если у задачи есть подзадачи, нужно убедиться, что они не будут добавлены повторно.
            if (task.SubTasks != null && task.SubTasks.Any())
            {
                foreach (var subTask in task.SubTasks)
                {
                    subTask.ParentTaskID = task.Id; // Убедимся, что у подзадач корректно проставлен ParentTaskID
                }
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(TodoTask task)
        {
            task.RegistrationDate = task.RegistrationDate.ToUniversalTime();
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                throw new InvalidOperationException("Задача не найдена.");

            //if (await HasSubTasksAsync(id))
            //    throw new InvalidOperationException("Невозможно удалить задачу с подзадачами.");

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
            var task = await _context.Tasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

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
