using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

using DMD.API.Controllers;
using DMD.Application.Interfaces;
using DMD.Application.DTOs;
using DMD.Persistence.Exceptions;
using DMD.Domain.Entities;

public class TaskControllerTests
{
    private readonly TaskController _taskController;
    private readonly Mock<ITaskService> _mockTaskService;

    public TaskControllerTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _taskController = new TaskController(_mockTaskService.Object);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOkWithTasks()
    {
        var tasks = new List<TaskDto>
        {
            new TaskDto { Id = 1, TaskName = "Task 1" },
            new TaskDto { Id = 2, TaskName = "Task 2" }
        };

        _mockTaskService.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

        var result = await _taskController.GetTasks();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(200);

        var returnedTasks = okResult?.Value as IEnumerable<TaskDto>;
        returnedTasks.Should().NotBeNull();
        returnedTasks?.Count().Should().Be(2);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreatedAtAction_WhenTaskIsCreated()
    {
        var newTask = new TodoTask { Id = 1, TaskName = "New Task" };

        _mockTaskService.Setup(s => s.CreateTaskAsync(It.IsAny<TodoTask>())).Returns(Task.CompletedTask);

        var result = await _taskController.CreateTask(newTask);

        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult?.StatusCode.Should().Be(201);
        createdResult?.ActionName.Should().Be("GetTask");

        var returnedTask = createdResult?.Value as TodoTask;
        returnedTask.Should().NotBeNull();
        returnedTask?.Id.Should().Be(newTask.Id);
        returnedTask?.TaskName.Should().Be(newTask.TaskName);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNoContent_WhenTaskIsUpdatedSuccessfully()
    {
        var taskToUpdate = new TodoTask { Id = 1, TaskName = "Updated Task" };

        _mockTaskService.Setup(s => s.UpdateTaskAsync(It.IsAny<TodoTask>())).Returns(Task.CompletedTask);

        var result = await _taskController.UpdateTask(taskToUpdate.Id, taskToUpdate);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskIsDeletedSuccessfully()
    {
        _mockTaskService.Setup(s => s.DeleteTaskAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        var result = await _taskController.DeleteTask(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangeTaskStatus_ShouldReturnBadRequest_WhenInvalidStatusIsGiven()
    {
        _mockTaskService.Setup(s => s.ChangeTaskStatusAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidTaskStatusException("Недопустимый статус задачи."));

        var result = await _taskController.ChangeTaskStatus(1, "InvalidStatus");

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult?.StatusCode.Should().Be(400);
        badRequestResult?.Value.Should().Be("Недопустимый статус задачи.");
    }
}
