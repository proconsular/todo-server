using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Constants;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers {
    /// <summary>
    /// Controller that provides basic CRUD operations for the <c>TodoItem<c> model.
    /// </summary>
    [Route(ControllerConstants.DefaultRoute)]
    [ApiController]
    public class TodoItemsController : ControllerBase {
        private readonly ILogger<TodoItemsController> _logger;
        private readonly ITodoItemsService _service;

        public TodoItemsController(ITodoItemsService service, ILogger<TodoItemsController> logger) {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all todo items.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <term>200 OK</term>
        /// <description>Returns a collection of todo items</description>
        /// </item>
        /// <item>
        /// <term>500 Internal Server Error</term>
        /// <description>If an error occurs while retrieving the items</description>
        /// </item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllItemsAsync() {
            try {
                var items = await _service.GetTodoItemsAsync();
                _logger.LogInformation(TodoControllerStrings.GET_ALL_ITEMS, items.Count());
                return Ok(items);
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_GET_ALL_ITEMS);
                return StatusCode(500, TodoControllerStrings.ERROR_GET_ALL_ITEMS);
            }
        }

        /// <summary>
        /// Retrieves a single todo item by ID.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// <term>200 OK</term>
        /// <description>Returns a single todo item</description>
        /// </item>
        /// <item>
        /// <term>500 Internal Server Error</term>
        /// <description>If an error occurs while retrieving the item</description>
        /// </item>
        /// </list>
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemAsync(int id) {
            try {
                var item = await _service.GetTodoItemAsync(id);
                if (item == null) {
                    return NotFound();
                }
                return Ok(item);
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_GET_ITEM, id);
                return StatusCode(500, TodoControllerStrings.EXC_GET_ITEM);
            }
        }

        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        /// <param name="item">The todo item to create</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><term>201 Created</term><description>Returns the created todo item</description></item>
        /// <item><term>500 Internal Server Error</term><description>If an error occurs while creating the item</description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateItemAsync([FromBody] TodoItem item) {
            try {
                var record = await _service.CreateTodoItemAsync(item);
                _logger.LogInformation(TodoControllerStrings.CREATE_ITEM, record.Id);
                return Created($"api/todoitems/{record.Id}", record);
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_CREATE_ITEM);
                return StatusCode(500, TodoControllerStrings.ERROR_CREATE_ITEM);
            }
        }

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        /// <param name="id">The ID of the todo item to update</param>
        /// <param name="item">The updated todo item data</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><term>200 OK</term><description>Returns updated item.</description></item>
        /// <item><term>400 Bad Request</term><description>If the ID in the URL doesn't match the item ID</description></item>
        /// <item><term>500 Internal Server Error</term><description>If an error occurs while updating the item</description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(int id, [FromBody] TodoItem item) {
            try {
                if (id != item.Id) {
                    return BadRequest();
                }
                var updatedItem = await _service.UpdateTodoItemAsync(item);
                _logger.LogInformation(TodoControllerStrings.UPDATE_ITEM, id);
                return Ok(updatedItem);
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_UPDATE_ITEM, id);
                return StatusCode(500, TodoControllerStrings.EXC_UPDATE_ITEM);
            }
        }

        /// <summary>
        /// Deletes a specific todo item by ID.
        /// </summary>
        /// <param name="id">The ID of the todo item to delete</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><term>204 No Content</term><description>If the item was successfully deleted</description></item>
        /// <item><term>404 Not Found</term><description>If the item doesn't exist</description></item>
        /// <item><term>500 Internal Server Error</term><description>If an error occurs while deleting the item</description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(int id) {
            try {
                bool deleted = await _service.DeleteTodoItemAsync(id);
                if (deleted) {
                    _logger.LogInformation(TodoControllerStrings.DELETE_ITEM, id);
                    return NoContent();
                } else {
                    return NotFound();
                }
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_DELETE_ITEM, id);
                return StatusCode(500, TodoControllerStrings.EXC_DELETE_ITEM);
            }
        }

        /// <summary>
        /// Deletes multiple todo items by their IDs.
        /// </summary>
        /// <param name="ids">Collection of todo item IDs to delete</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><term>200 OK</term><description>Returns the list of successfully deleted item IDs</description></item>
        /// <item><term>500 Internal Server Error</term><description>If an error occurs while deleting the items</description></item>
        /// </list>
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteItemsAsync([FromBody] IEnumerable<int> ids) {
            try {
                var deletedIds = await _service.DeleteTodoItemsAsync(ids);
                _logger.LogInformation(TodoControllerStrings.DELETE_ITEMS, deletedIds.Count());
                return Ok(deletedIds);
            } catch (Exception ex) {
                _logger.LogError(ex, TodoControllerStrings.ERROR_DELETE_ITEMS);
                return StatusCode(500, TodoControllerStrings.ERROR_DELETE_ITEMS);
            }
        }
    }
}