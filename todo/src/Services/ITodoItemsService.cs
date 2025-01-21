using Todo.Models;

namespace Todo.Services {
    /// <summary>
    /// Defines the contract for a service that manages Todo items.
    /// </summary>
    public interface ITodoItemsService 
    {
        /// <summary>
        /// Retrieves all todo items.
        /// </summary>
        /// <returns>A collection of all todo items.</returns>
        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();

        /// <summary>
        /// Retrieves a single todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the todo item to retrieve.</param>
        /// <returns>The requested todo item if found; otherwise, null.</returns>
        Task<TodoItem?> GetTodoItemAsync(int id);

        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        /// <param name="item">The todo item to create.</param>
        /// <returns>The created todo item with its ID.</returns>
        /// <remarks>
        /// The service will set the CreatedAt and UpdatedAt timestamps automatically.
        /// </remarks>
        Task<TodoItem> CreateTodoItemAsync(TodoItem item);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        /// <param name="item">The todo item with updated values.</param>
        /// <remarks>
        /// The service will update the UpdatedAt timestamp automatically.
        /// The item's ID must match an existing Todo item.
        /// </remarks>
        Task<TodoItem> UpdateTodoItemAsync(TodoItem item);

        /// <summary>
        /// Deletes a single todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to delete.</param>
        /// <returns>True if the item was successfully deleted; false if the item was not found.</returns>
        Task<bool> DeleteTodoItemAsync(int id);

        /// <summary>
        /// Deletes multiple todo items.
        /// </summary>
        /// <param name="ids">The collection of ID of the Todo items to delete.</param>
        /// <returns>A collection of IDs that were successfully deleted.</returns>
        /// <remarks>
        /// IDs that don't exist in the system will be ignored.
        /// </remarks>
        Task<IEnumerable<int>> DeleteTodoItemsAsync(IEnumerable<int> ids);
    }
}