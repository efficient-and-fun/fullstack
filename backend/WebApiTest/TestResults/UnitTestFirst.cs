// using Microsoft.AspNetCore.Mvc;
// using WebApi;
//
// namespace WebApiTest;
//
// [TestClass]
// public class UnitTestFirst
// {
//     private MeetUpController _controller;
//
//     [TestInitialize]
//     public void Initialize()
//     {
//         _controller = new FirstController();
//     }
//
//     [TestMethod]
//     public void TestEcho()
//     {
//         // Arrange
//         var expected = "Test message";
//         var dto = new FirstController.EchoDto 
//         { 
//             Message = expected 
//         };
//
//         // Act
//         var result = _controller.Echo(dto);
//         
//         // Assert
//         var okResult = result.Result as OkObjectResult;
//         Assert.IsNotNull(okResult, "Expected an OkObjectResult");
//         
//         var actual = okResult.Value as string;
//         Assert.IsNotNull(actual, "Expected a string");
//         
//         Assert.AreEqual(expected, actual);
//     }
// }