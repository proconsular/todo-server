using Microsoft.AspNetCore.Mvc;
using Todo.Controllers;
using Todo.Models;
using Todo.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Todo.Constants;

/// <summary>
/// Tests for <c>TodoItemsController</c> class.
/// </summary>
public class TodoItemsControllerTests
{
    private readonly Mock<ITodoItemsService> _mockService;
    private readonly TodoItemsController _controller;
    private readonly Mock<ILogger<TodoItemsController>> _mockLogger;

    /// <summary>
    /// Sets up mock items service and logger.
    /// </summary>
    public TodoItemsControllerTests()
    {
        _mockService = new Mock<ITodoItemsService>();
        _mockLogger = new Mock<ILogger<TodoItemsController>>();
        _controller = new TodoItemsController(_mockService.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Verifies GetAllItemsAsync returns 200 Ok with expected items if no exceptions were thrown.
    /// </summary>
    [Fact]
    public async Task GetAllItems_ReturnsOkResult()
    {
        var expectedItems = new List<TodoItem> 
        { 
            new TodoItem { Id = 1, Title = "Create new task" },
            new TodoItem { Id = 3, Title = "Write tests" } 
        };
        _mockService
            .Setup(s => s.GetTodoItemsAsync())
            .ReturnsAsync(expectedItems);

        var result = await _controller.GetAllItemsAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
        Assert.Equal(2, items.Count());

        Assert.Equal(1, items.ElementAt(0).Id);
        Assert.Equal("Create new task", items.ElementAt(0).Title);
        Assert.Equal(3, items.ElementAt(1).Id);
        Assert.Equal("Write tests", items.ElementAt(1).Title);

        VerifyLoggerWasCalled("Retrieved 2 todo items");
    }

    /// <summary>
    /// Verifies GetAllItemsAsync logs exception and returns 500 InternalServerError with proper message.
    /// </summary>
    [Fact]
    public async Task GetAllItems_Returns500Error()
    {
        _mockService
            .Setup(s => s.GetTodoItemsAsync())
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.GetAllItemsAsync();

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, errorResult.StatusCode);
        Assert.Equal(TodoControllerStrings.ERROR_GET_ALL_ITEMS, errorResult.Value);
        VerifyLoggerWasCalled("Error retrieving todo items", LogLevel.Error);
    }

    /// <summary>
    /// Verifies GetItemAsync returns 200 Ok with item by ID.
    /// </summary>
    [Fact]
    public async Task GetItem_ReturnsItem()
    {
        _mockService
            .Setup(s => s.GetTodoItemAsync(2))
            .ReturnsAsync(new TodoItem { Id = 2, Title = "Hello world"});

        var result = await _controller.GetItemAsync(2);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var item = Assert.IsType<TodoItem>(okResult.Value);
        Assert.Equal(2, item.Id);
        Assert.Equal("Hello world", item.Title);
    }

    /// <summary>
    /// Verifies GetItemAsync returns 404 NotFound status if item is not found.
    /// </summary>
    [Fact]
    public async Task GetItem_NotFound_ReturnsNotFound()
    {
        _mockService
            .Setup(s => s.GetTodoItemAsync(1))
            .ReturnsAsync((TodoItem)null);

        var result1 = await _controller.GetItemAsync(1);

        Assert.IsType<NotFoundResult>(result1);
    }

    /// <summary>
    /// Verifies GetItemAsync logs exception and returns 500 InternalServerError with proper message.
    /// </summary>
    [Fact]
    public async Task GetItem_Failed_Returns500Error()
    {
        _mockService
            .Setup(s => s.GetTodoItemAsync(2))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.GetItemAsync(2);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal(TodoControllerStrings.EXC_GET_ITEM, statusCodeResult.Value);
        VerifyLoggerWasCalled("Error retrieving todo item with id 2", LogLevel.Error);
    }

    /// <summary>
    /// Verifies CreateItemAsync returns 201 Created with item and logs event with proper message.
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ValidItem_ReturnsCreatedResult()
    {
        var todoItem = new TodoItem { Title = "Test Item" };
        var expectedItem = new TodoItem { Id = 43, Title = "Test Item" };
        _mockService
            .Setup(s => s.CreateTodoItemAsync(todoItem))
            .ReturnsAsync(expectedItem);

        var result = await _controller.CreateItemAsync(todoItem);

        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal($"api/todoitems/43", createdResult.Location);
        var createdItem = Assert.IsType<TodoItem>(createdResult.Value);
        Assert.Equal(expectedItem.Id, createdItem.Id);
        Assert.Equal(expectedItem.Title, createdItem.Title);
        VerifyLoggerWasCalled("Created todo item with id 43");
    }

    /// <summary>
    /// Verifites CreateItemAsync returns 500 InternalServerError and logs exception with proper message if exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateItemAsync_ServiceThrowsException_Returns500Error()
    {
        var todoItem = new TodoItem { Id = 1, Title = "Test Item" };
        _mockService
            .Setup(s => s.CreateTodoItemAsync(todoItem))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.CreateItemAsync(todoItem);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal(TodoControllerStrings.ERROR_CREATE_ITEM, statusCodeResult.Value);
        VerifyLoggerWasCalled("Error creating todo item", LogLevel.Error);
    }

    /// <summary>
    /// Verifies UpdateItemAsync returns 200 with updated item if item is valid and logs event with proper message.
    /// </summary>
    [Fact]
    public async Task UpdateItemAsync_ValidItem_ReturnsOKwithItems()
    {
        var todoItem = new TodoItem { Id = 1, Title = "Updated Item" };
        _mockService
            .Setup(s => s.UpdateTodoItemAsync(todoItem))
            .ReturnsAsync(todoItem);

        var result = await _controller.UpdateItemAsync(1, todoItem);

        Assert.IsType<OkObjectResult>(result);
        _mockService.Verify(s => s.UpdateTodoItemAsync(todoItem), Times.Once);

        VerifyLoggerWasCalled("Updated todo item with id 1");
    }

    /// <summary>
    /// Verifies UpdateItemAsync returns 400 BadRequest if ID in route doesn't match item in body.
    /// </summary>
    [Fact]
    public async Task UpdateItemAsync_IdMismatch_ReturnsBadRequest()
    {
        var todoItem = new TodoItem { Id = 2, Title = "Updated Item" };

        var result = await _controller.UpdateItemAsync(1, todoItem);

        Assert.IsType<BadRequestResult>(result);
        _mockService.Verify(s => s.UpdateTodoItemAsync(It.IsAny<TodoItem>()), Times.Never);
    }

    /// <summary>
    /// Verifies UpdateItemAsync returns 500 InternalServerError and logs exception with proper message when exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateItemAsync_ServiceThrowsException_Returns500Error()
    {
        var todoItem = new TodoItem { Id = 1, Title = "Updated Item" };
        _mockService
            .Setup(s => s.UpdateTodoItemAsync(todoItem))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.UpdateItemAsync(1, todoItem);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal(TodoControllerStrings.EXC_UPDATE_ITEM, statusCodeResult.Value);
        VerifyLoggerWasCalled("Error updating todo item with id 1", LogLevel.Error);
    }

    /// <summary>
    /// Verifies DeleteItemAsync returns 204 NoContent when item is deleted and logs events with proper message.
    /// </summary>
    [Fact]
    public async Task DeleteItemAsync_ExistingItem_ReturnsNoContent()
    {
        int id = 1;
        _mockService
            .Setup(s => s.DeleteTodoItemAsync(id))
            .ReturnsAsync(true);

        var result = await _controller.DeleteItemAsync(id);

        Assert.IsType<NoContentResult>(result);
        VerifyLoggerWasCalled("Deleted todo item with id 1");
    }

    /// <summary>
    /// Verifies DeleteItemAsync returns 404 NotFound if item is not found in repository.
    /// </summary>
    [Fact]
    public async Task DeleteItemAsync_NonExistingItem_ReturnsNotFound()
    {
        int id = 1;
        _mockService
            .Setup(s => s.DeleteTodoItemAsync(id))
            .ReturnsAsync(false);

        var result = await _controller.DeleteItemAsync(id);

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Verifies DeleteItemAsync returns 500 InternalServerError and logs exception with proper message when exception is thrown.
    /// </summary>
    [Fact]
    public async Task DeleteItemAsync_ServiceThrowsException_Returns500Error()
    {
        int id = 1;
        _mockService
            .Setup(s => s.DeleteTodoItemAsync(id))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.DeleteItemAsync(id);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal(TodoControllerStrings.EXC_DELETE_ITEM, statusCodeResult.Value);
        VerifyLoggerWasCalled("Error deleting todo item with id 1", LogLevel.Error);
    }

    /// <summary>
    /// Verifies DeleteItemAsync returns 200 Ok with list of IDs of the items removed and logs event with proper message.
    /// </summary>
    [Fact]
    public async Task DeleteItemsAsync_ValidIds_ReturnsOkWithDeletedIds()
    {
        var ids = new[] { 1, 2, 3 };
        var deletedIds = new[] { 1, 2 };
        _mockService.Setup(s => s.DeleteTodoItemsAsync(ids)).ReturnsAsync(deletedIds);

        var result = await _controller.DeleteItemsAsync(ids);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(deletedIds, okResult.Value);
        VerifyLoggerWasCalled("Deleted 2 todo items");
    }

    /// <summary>
    /// Verifies DeleteItemsAsync retursn 500 InternalServerError and logs exception with proper message when exception is thrown.
    /// </summary>
    [Fact]
    public async Task DeleteItemsAsync_ServiceThrowsException_Returns500Error()
    {
        var ids = new[] { 1, 2, 3 };
        _mockService.Setup(s => s.DeleteTodoItemsAsync(ids))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _controller.DeleteItemsAsync(ids);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal(TodoControllerStrings.ERROR_DELETE_ITEMS, statusCodeResult.Value);
        VerifyLoggerWasCalled("Error deleting todo items", LogLevel.Error);
    }

    /// <summary>
    /// Helper method for verifying log output.
    /// </summary>
    /// <param name="expectedMessage">Log message expected to be logged.</param>
    /// <param name="logLevel">Log level, default is Info</param>
    private void VerifyLoggerWasCalled(string expectedMessage, LogLevel logLevel = LogLevel.Information)
    {
        Func<object, Type, bool> state = (v, t) => {
            var actualMessage = v.ToString();
            return actualMessage.Contains(expectedMessage);
        };
    
        _mockLogger.Verify(x => x.Log(
            It.Is<LogLevel>(l => l == logLevel),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => state(v, t)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true))
        );
    }
}