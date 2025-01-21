using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Todo.Controllers;

/// <summary>
/// Tests for Health Controller. Basically a sanity test.
/// </summary>
public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public void  Check_ReturnsOk()
    {
        var result = _controller.Check();
        Assert.IsType<OkResult>(result);
    }
}
