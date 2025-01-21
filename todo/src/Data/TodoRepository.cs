
using Microsoft.EntityFrameworkCore;
using Todo.Constants;
using Todo.Models;

namespace Todo.Data {
    /// <summary>
    /// Repository implementation for managing TodoItem entities in the database.
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly ITodoDbContext _context;

        public TodoRepository(ITodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem?> GetByIdAsync(int id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task AddAsync(TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id != 0)
                throw new InvalidOperationException(TodoRepositoryStrings.EXC_CREATE_ITEM_ID_ZERO);

            await _context.TodoItems.AddAsync(item);
        }

        public async Task<TodoItem> UpdateAsync(TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            var existingItem = await _context.TodoItems.FindAsync(item.Id);
            if (existingItem == null)
                throw new InvalidOperationException(string.Format(TodoRepositoryStrings.EXC_UPDATE_ITEM_NOT_FOUND, item.Id));

            if (existingItem.Title == item.Title && existingItem.IsDone == item.IsDone)
                return existingItem;

            _context.Entry(existingItem).CurrentValues.SetValues(item);

            return existingItem;
        }

        public async Task<IEnumerable<TodoItem>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<TodoItem>();
            return await _context.TodoItems.Where(item => ids.Contains(item.Id)).ToListAsync();
        }

        public Task RemoveRangeAsync(IEnumerable<TodoItem> items)
        {
            if (items == null || !items.Any())
                return Task.CompletedTask;
            _context.TodoItems.RemoveRange(items);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}