using Api.Controllers.Api;
using Api.Models.Data;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;


namespace Api.Tests.UnitTests.Controllers
{
    public class UserControllerTests
    {

        private readonly Mock<IUserService> _serviceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _serviceMock = new Mock<IUserService>();
            _controller = new UserController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkAndAListOfUsers()
        {
            // Arrange
            var testUsers = new List<User>
            {
                new User { Id = 1, Email = "ashtonb", Roles = new List<string> { "Admin" } },
                new User { Id = 2, Email = "guest_mike", Roles = new List<string> { "Guest" } }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(testUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetById_ReturnsOkAndAUser()
        {
            // Arrange
            var testUsers = new List<User>
            {
                new User { Id = 1, Email = "ashtonb", Roles = new List<string> { "Admin" } },
                new User { Id = 2, Email = "guest_mike", Roles = new List<string> { "Guest" } }
            };

            int userIdToTest = 2;
            _serviceMock.Setup(s => s.GetByIdAsync(userIdToTest)).ReturnsAsync(testUsers.Find((u) => u.Id == userIdToTest));

            // Act
            var result = await _controller.GetById(userIdToTest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user= Assert.IsAssignableFrom<User>(okResult.Value);
            Assert.Equal(userIdToTest, user.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int nonExistentUserId = 999;

            _serviceMock.Setup(s => s.GetByIdAsync(nonExistentUserId))
                        .ReturnsAsync((User?)null);

            // Act
            var actionResult = await _controller.GetById(nonExistentUserId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
