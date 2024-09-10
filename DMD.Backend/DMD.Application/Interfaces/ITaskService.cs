using DMD.Application.DTOs;
using DMD.Domain.Entities;

namespace DMD.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task CreateTaskAsync(TodoTask task);
        Task UpdateTaskAsync(TodoTask task);
        Task DeleteTaskAsync(int id);
        Task ChangeTaskStatusAsync(int taskId, string status);
    }
}
