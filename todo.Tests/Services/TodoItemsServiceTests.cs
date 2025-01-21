using Moq;
using Todo.Data;
using Todo.Models;
using Todo.Services;

/// <summary>
/// Tests for <c>TodoItemsService</c> class.
/// </summary>
public class TodoItemsServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepository;
    private readonly ITodoItemsService _service;
    private readonly List<TodoItem> _data;

    /// <summary>
    /// Sets up repository with some test data.
    /// </summary>
    public TodoItemsServiceTests()
    {
        _data = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Test Item 1" },
            new TodoItem { Id = 2, Title = "Test Item 2" },
            new TodoItem { Id = 3, Title = "Test Item 3" }
        };

        _mockRepository = new Mock<ITodoRepository>();
        _service = new TodoItemsService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies GetTodoItemAsync returns all items in repository.
    /// </summary>
    [Fact]
    public async Task GetTodoItemsAsync_ShouldReturnAllItems()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(_data);

        // Act
        var result = await _service.GetTodoItemsAsync();

        // Assert
        Assert.Equal(_data.Count, result.Count());
        for (int i = 0; i < _data.Count; i++)
        {
            Assert.Equal(_data[i].Id, result.ElementAt(i).Id);
            Assert.Equal(_data[i].Title, result.ElementAt(i).Title);
        }
    }

    /// <summary>
    /// Verifies GetTodoItemAsync returns item by ID when ID is valid.
    /// </summary>
    [Fact]
    public async Task GetTodoItemAsync_WithValidId_ShouldReturnItem()
    {
        // Arrange
        var expectedItem = _data.First();
        _mockRepository.Setup(r => r.GetByIdAsync(expectedItem.Id))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetTodoItemAsync(expectedItem.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedItem.Id, result.Id);
        Assert.Equal(expectedItem.Title, result.Title);
    }

    /// <summary>
    /// Verifies GetTodoItemAsync returns Null when item is not found.
    /// </summary>
    [Fact]
    public async Task GetTodoItemAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((TodoItem)null);

        // Act
        var result = await _service.GetTodoItemAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies CreateTodoItemAsync returns new item and the correct repository methods are called.
    /// </summary>
    [Fact]
    public async Task CreateTodoItemAsync_ShouldAddAndReturnItem()
    {
        // Arrange
        var newItem = new TodoItem { Id = 4, Title = "New Test Item" };

        // Act
        var result = await _service.CreateTodoItemAsync(newItem);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(newItem), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal(newItem.Id, result.Id);
        Assert.Equal(newItem.Title, result.Title);
        Assert.Equal(newItem.IsDone, result.IsDone);
    }

    /// <summary>
    /// Verifies UpdateTodoItemAsync calls the correct repository methods to update the item.
    /// </summary>
    [Fact]
    public async Task UpdateTodoItemAsync_ShouldUpdateItem()
    {
        // Arrange
        var item = _data.First();
        item.Title = "Updated Title";

        // Act
        await _service.UpdateTodoItemAsync(item);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(item), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies DeleteTodoItemAsync returns True and calls correct repository methods to delete item with valid ID.
    /// </summary>
    [Fact]
    public async Task DeleteTodoItemAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var itemToDelete = _data.First();
        _mockRepository.Setup(r => r.GetByIdsAsync(It.Is<IEnumerable<int>>(ids => ids.Single() == itemToDelete.Id)))
            .ReturnsAsync([itemToDelete]);

        // Act
        var result = await _service.DeleteTodoItemAsync(itemToDelete.Id);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.RemoveRangeAsync(It.Is<IEnumerable<TodoItem>>(items => 
            items.Single().Id == itemToDelete.Id)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies DeleteTodoItemAsync returns False if ID is not found and repository methods are not called.
    /// </summary>
    [Fact]
    public async Task DeleteTodoItemAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdsAsync(It.Is<IEnumerable<int>>(ids => ids.Single() == 999)))
            .ReturnsAsync(Enumerable.Empty<TodoItem>());

        // Act
        var result = await _service.DeleteTodoItemAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<TodoItem>>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies DeleteTodoItemsAsync returns deleted IDs and calls correct repository methods.
    /// </summary>
    [Fact]
    public async Task DeleteTodoItemsAsync_WithValidIds_ShouldReturnDeletedIds()
    {
        // Arrange
        var itemsToDelete = _data.Take(2).ToList();
        var ids = itemsToDelete.Select(x => x.Id);
        _mockRepository.Setup(r => r.GetByIdsAsync(It.Is<IEnumerable<int>>(x => x.SequenceEqual(ids))))
            .ReturnsAsync(itemsToDelete);

        // Act
        var result = await _service.DeleteTodoItemsAsync(ids);

        // Assert
        Assert.Equal(ids, result);
        _mockRepository.Verify(r => r.RemoveRangeAsync(It.Is<IEnumerable<TodoItem>>(items => 
            items.Count() == itemsToDelete.Count)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies DeleteTodoItemsAsync returns empty list if IDs provided are invalid and repository methods are not called.
    /// </summary>
    [Fact]
    public async Task DeleteTodoItemsAsync_WithInvalidIds_ShouldReturnEmptyList()
    {
        // Arrange
        var invalidIds = new[] { 999, 1000 };
        _mockRepository.Setup(r => r.GetByIdsAsync(It.Is<IEnumerable<int>>(x => x.SequenceEqual(invalidIds))))
            .ReturnsAsync(Enumerable.Empty<TodoItem>());

        // Act
        var result = await _service.DeleteTodoItemsAsync(invalidIds);

        // Assert
        Assert.Empty(result);
        _mockRepository.Verify(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<TodoItem>>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}