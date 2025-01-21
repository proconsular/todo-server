
namespace Todo.Constants {
    /// <summary>
    /// General constants for controllers
    /// </summary>
    public static class ControllerConstants {
        public const string DefaultRoute = "api/[controller]";
    }

    /// <summary>
    /// Todo Controller strings used for logging and exceptions.
    /// </summary>
    public static class TodoControllerStrings {
        public const string GET_ALL_ITEMS = "Retrieved {count} todo items";
        public const string ERROR_GET_ALL_ITEMS = "Error retrieving todo items";
        public const string ERROR_GET_ITEM = "Error retrieving todo item with id {id}";
        public const string EXC_GET_ITEM = "An error occurred while retrieving todo item";
        public const string CREATE_ITEM = "Created todo item with id {id}";
        public const string ERROR_CREATE_ITEM = "Error creating todo item";
        public const string UPDATE_ITEM = "Updated todo item with id {id}";
        public const string ERROR_UPDATE_ITEM = "Error updating todo item with id {id}";
        public const string EXC_UPDATE_ITEM = "An error occurred while updating todo item";
        public const string DELETE_ITEM = "Deleted todo item with id {id}";
        public const string ERROR_DELETE_ITEM = "Error deleting todo item with id {id}";
        public const string EXC_DELETE_ITEM = "An error occurred while deleting todo item";
        public const string DELETE_ITEMS = "Deleted {count} todo items";
        public const string ERROR_DELETE_ITEMS = "Error deleting todo items";
    }

    /// <summary>
    /// Todo Repository strings used for exceptions.
    /// </summary>
    public static class TodoRepositoryStrings {
        public const string EXC_CREATE_ITEM_ID_ZERO = "TodoItem.Id must be 0 when adding a new item";
        public const string EXC_UPDATE_ITEM_NOT_FOUND = "TodoItem with id {0} was not found";
    }
}