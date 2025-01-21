using System.ComponentModel.DataAnnotations;

namespace Todo.Models {
    /// <summary>
    /// Represents a todo item.
    /// </summary>
    public class TodoItem: IEntity {
        /// <summary>
        /// Gets or sets the unique identifier for the Todo item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the Todo item.
        /// </summary>
        /// <remarks>
        /// The title is required and must not exceed 40 characters in length.
        /// </remarks>
        [MaxLength(40)]
        [Required]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the todo item is completed.
        /// </summary>
        /// <remarks>
        /// Defaults to false when a new todo item is created.
        /// </remarks>
        public bool IsDone { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the todo item was created.
        /// </summary>
        /// <remarks>
        /// Automatically set to the current date and time when a new todo item is created.
        /// </remarks>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the todo item was last updated.
        /// </summary>
        /// <remarks>
        /// Initially set to the creation time and should be updated whenever the item is modified.
        /// </remarks>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}