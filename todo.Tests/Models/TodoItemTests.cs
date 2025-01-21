
using System.ComponentModel.DataAnnotations;
using Todo.Models;

/// <summary>
/// Tests TodoItem validations and defaults.
/// </summary>
public class TodoItemTests {

    /// <summary>
    /// Tests defaults of a new item.
    /// </summary>
    [Fact]
    public void TodoItem_NewInstance_HasDefaultValues()
    {
        var todo = new TodoItem();
        var currentTime = DateTime.Now;

        Assert.Equal(0, todo.Id);
        Assert.Null(todo.Title);
        Assert.False(todo.IsDone);
        Assert.True((currentTime - todo.CreatedAt).TotalSeconds < 1);
        Assert.True((currentTime - todo.UpdatedAt).TotalSeconds < 1);
    }

    /// <summary>
    /// Verifies Title property is valid <= 20 chars
    /// </summary>
    [Theory]
    [InlineData("Valid Title")]
    [InlineData("A")]
    [InlineData("Twenty Chars Here!!!")]  // Exactly 20 characters
    public void TodoItem_ValidTitle_PassesValidation(string title)
    {
        var todo = new TodoItem { Title = title };
        var validationContext = new ValidationContext(todo);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(todo, validationContext, validationResults, true);

        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    /// <summary>
    /// Verifies Title maximum length validation.
    /// </summary>
    [Theory]
    [InlineData("This title is way too long and should fail validation", "The field Title must be a string or array type with a maximum length of '40'.")]
    public void TodoItem_InvalidTitle_FailsValidation(string title, string expectedError)
    {
        var todo = new TodoItem { Title = title };
        var validationContext = new ValidationContext(todo);
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(todo, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Contains(expectedError, validationResults.First().ErrorMessage);
    }
}