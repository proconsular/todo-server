using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Models;

namespace Todo.Services {
    public class TodoItemsService: ITodoItemsService {
        private readonly ITodoRepository _repository;
        
        public TodoItemsService(ITodoRepository repository) {
            _repository = repository;
        }
        
        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync() {
            return await _repository.GetAllAsync();
        }

        public async Task<TodoItem?> GetTodoItemAsync(int id) {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<TodoItem> CreateTodoItemAsync(TodoItem item) {
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();
            return item!;
        }

        public async Task<TodoItem> UpdateTodoItemAsync(TodoItem item) {
            var updatedItem = await _repository.UpdateAsync(item);
            await _repository.SaveChangesAsync();
            return updatedItem;
        }

        public async Task<bool> DeleteTodoItemAsync(int id) {
            var result = await DeleteTodoItemsAsync([id]);
            return result.Any();
        }

        public async Task<IEnumerable<int>> DeleteTodoItemsAsync(IEnumerable<int> ids) {
            var items = await _repository.GetByIdsAsync(ids);
            var itemsList = items.ToList();
            
            if (!itemsList.Any())
                return Enumerable.Empty<int>();
            
            await _repository.RemoveRangeAsync(itemsList);
            await _repository.SaveChangesAsync();
            return itemsList.Select(item => item.Id);
        }
    }
}