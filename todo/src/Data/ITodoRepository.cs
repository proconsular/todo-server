
using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data {
    /// <summary>
    /// Provides data access operations for TodoItem entities.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Retrieves all TodoItem entities asynchronously.
        /// </summary>
        /// <returns>A collection of all TodoItem entities.</returns>
        Task<IEnumerable<TodoItem>> GetAllAsync();

        /// <summary>
        /// Retrieves a TodoItem by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the TodoItem.</param>
        /// <returns>The TodoItem if found; otherwise, null.</returns>
        Task<TodoItem?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new TodoItem to the repository asynchronously.
        /// </summary>
        /// <param name="item">The TodoItem to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when item is null.</exception>
        Task AddAsync(TodoItem item);

        /// <summary>
        /// Updates an existing TodoItem in the repository asynchronously.
        /// </summary>
        /// <param name="item">The TodoItem to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when item is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the item doesn't exist in the repository.</exception>
        Task<TodoItem> UpdateAsync(TodoItem item);

        /// <summary>
        /// Retrieves multiple TodoItems by their IDs asynchronously.
        /// </summary>
        /// <param name="ids">The collection of IDs to search for.</param>
        /// <returns>A collection of found TodoItems. Items that weren't found will be excluded from the result.</returns>
        Task<IEnumerable<TodoItem>> GetByIdsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Removes multiple TodoItems from the repository asynchronously.
        /// </summary>
        /// <param name="items">The collection of TodoItems to remove.</param>
        Task RemoveRangeAsync(IEnumerable<TodoItem> items);

        /// <summary>
        /// Saves all changes made in this repository to the underlying data store asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveChangesAsync();
    }
}