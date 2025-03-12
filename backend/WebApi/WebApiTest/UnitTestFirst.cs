using Microsoft.AspNetCore.Mvc;
using WebApi;

namespace WebApiTest;

[TestClass]
public class UnitTestFirst
{
    private FirstController _controller;

    [TestInitialize]
    public void Initialize()
    {
        _controller = new FirstController();
    }
    
    [TestMethod]
    public void TestHello()
    {
        var expected = "Hello";

        var result = _controller.GetHello();
        
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected an OkObjectResult");
        
        var actual = okResult.Value as string;
        Assert.IsNotNull(actual, "Expected a string");
        
        Assert.AreEqual(expected, actual);
    }
}