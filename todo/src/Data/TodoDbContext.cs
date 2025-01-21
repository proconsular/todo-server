using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data {
    /// <summary>
    /// Database context implementation for Todo items using Entity Framework Core.
    /// </summary>
    public class TodoDbContext: DbContext, ITodoDbContext {
        public DbSet<TodoItem> TodoItems { get; set; }

        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) {}

        public override async Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is IEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((IEntity)entity.Entity).CreatedAt = now;
                }
                ((IEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}