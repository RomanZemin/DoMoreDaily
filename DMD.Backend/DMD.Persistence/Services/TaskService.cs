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
            // Загружаем все задачи и подзадачи одним запросом
            var allTasks = await _context.Tasks
                .AsNoTracking()
                .ToListAsync();

            // Рекурсивный метод для маппинга подзадач
            TaskDto MapTask(TodoTask task, List<TodoTask> allTasks)
            {
                var taskDto = _mapper.Map<TaskDto>(task);

                // Найти подзадачи для текущей задачи
                var subTasks = allTasks.Where(t => t.ParentTaskID == task.Id).ToList();

                // Если подзадачи найдены, рекурсивно маппим их
                if (subTasks.Any())
                {
                    taskDto.SubTasks = subTasks.Select(subTask => MapTask(subTask, allTasks)).ToList();
                }

                return taskDto;
            }

            // Ищем все верхнеуровневые задачи (без ParentTaskID)
            var rootTasks = allTasks
                .Where(t => t.ParentTaskID == null)
                .OrderBy(t => t.TaskName)
                .ToList();

            // Обрабатываем все верхнеуровневые задачи и рекурсивно маппим подзадачи
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
            // Проверяем, если дата не в UTC и у неё неопределённый Kind, конвертируем в UTC
            if (task.RegistrationDate.Kind == DateTimeKind.Unspecified)
            {
                // Предположим, что неуказанный Kind - это местное время, конвертируем в UTC
                task.RegistrationDate = DateTime.SpecifyKind(task.RegistrationDate, DateTimeKind.Local).ToUniversalTime();
            }
            else if (task.RegistrationDate.Kind == DateTimeKind.Local)
            {
                // Если это местное время, конвертируем его в UTC
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

            //if (await HasSubTasksAsync(id))
            //    throw new InvalidOperationException("Невозможно удалить задачу с подзадачами.");

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

            // Проверка на переходы между статусами
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
                // Прямое изменение статуса (например, Назначена -> Выполняется)
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
