using Moq;
using Api.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Tests.UnitTests.Controllers
{
    public class ConnectionControllerTests
    {
        private readonly ConnectionController _controller;

        public ConnectionControllerTests()
        {
            _controller = new ConnectionController();
        }

        [Fact]
        public void Get_ReturnsOkResultWithExpectedMessage()
        {
            // Act
            var result = _controller.Get();

            // Assert
            Assert.Contains("Connected.", "Connected.");
            var okResult = Assert.IsType<OkObjectResult>(result);
            string json = JsonSerializer.Serialize(okResult.Value);
            Assert.Contains("Connected.", json);
        }
    }
}
