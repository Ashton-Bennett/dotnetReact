using Moq;
using Microsoft.Extensions.Logging;
using reactTestApp.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api.Tests.UnitTests.Controllers
{
    public class ConnectionControllerTests
    {
        private readonly Mock<ILogger<ConnectionController>> _loggerMock;
        private readonly ConnectionController _controller;

        public ConnectionControllerTests()
        {
            _loggerMock = new Mock<ILogger<ConnectionController>>();
            _controller = new ConnectionController(_loggerMock.Object);
        }

        [Fact]
        public void Get_ReturnsOkResultWithExpectedMessage()
        {
            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            string json = JsonSerializer.Serialize(okResult.Value);
            Assert.Contains("Connected.", json);
        }

        [Fact]
        public void Get_LogsInformation()
        {
            // Act
            _controller.Get();

            // Assert - Verify logger called once
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, _) => obj.ToString()!.Contains("Initial backend test message sent")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
