using Microsoft.AspNetCore.Mvc;

using DMD.Application.Interfaces;
using DMD.Domain.Entities;
using DMD.Persistence.Exceptions;

namespace DMD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        #region HTTPGet
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            if (tasks == null || !tasks.Any()) throw new TaskNotFoundException("Задачи не найдены.");
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) throw new TaskNotFoundException("Задача не найдена.");
            return Ok(task);
        }
        #endregion

        #region HTTPPost
        [HttpPost]
        public async Task<IActionResult> CreateTask(TodoTask task)
        {
            try
            {
                await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при создании задачи: " + ex.Message);
            }
        }
        #endregion

        #region HTTPPut
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TodoTask task)
        {
            if (id != task.Id) throw new InvalidOperationException("ID задачи не совпадает с ID в теле запроса.");

            try
            {
                await _taskService.UpdateTaskAsync(task);
                return NoContent();
            }
            catch (TaskNotFoundException ex)
            {
                throw new TaskNotFoundException($"Задача с ID {id} не найдена для обновления. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при обновлении задачи: " + ex.Message);
            }
        }
        #endregion

        #region HTTPDelete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (TaskNotFoundException ex)
            {
                throw new TaskNotFoundException($"Задача с ID {id} не найдена для удаления. " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Ошибка при удалении задачи: {ex.Message}");
            }
        }
        #endregion

        #region HTTPPatch
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeTaskStatus(int id, [FromBody] string status)
        {
            try
            {
                await _taskService.ChangeTaskStatusAsync(id, status);
                return NoContent();
            }
            catch (TaskNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidTaskStatusException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        #endregion
    }

}
