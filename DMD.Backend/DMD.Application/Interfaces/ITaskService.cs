using DMD.Domain.Entities;

namespace DMD.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<TodoTask> GetTaskByIdAsync(int id);
        Task CreateTaskAsync(TodoTask task);
        Task UpdateTaskAsync(TodoTask task);
        Task DeleteTaskAsync(int id);
        Task ChangeTaskStatusAsync(int taskId, string status);
    }
}
