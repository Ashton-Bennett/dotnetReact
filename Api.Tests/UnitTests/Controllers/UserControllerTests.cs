using Api.Controllers.Api;
using Api.Enums;
using Api.Models.Data;
using Api.Models.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
        public async Task GetAll_WhenServiceReturnsSuccess_ReturnsOkWithUsers()
        {
            // Arrange
            var testUsers = new List<User>
            {
                new User { Id = 1, Email = "ashtonb", Roles = new List<string> { "Admin" } },
                new User { Id = 2, Email = "guest_mike", Roles = new List<string> { "Guest" } }
            };

            // Mock the service to return a successful ServiceResult
            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<User>>.Ok(testUsers));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var successResponse = Assert.IsType<ApiSuccessResponse<IEnumerable<User>>>(okResult.Value);
            var users = successResponse.Data;

            Assert.NotNull(users);
            Assert.Equal(2, users.Count());
            Assert.Contains(users, u => u.Id == 1 && u.Email == "ashtonb");
            Assert.Contains(users, u => u.Id == 2 && u.Email == "guest_mike");
        }

        [Fact]
        public async Task GetAll_WhenServiceFails_ReturnsErrorMessage()
        {
            // Arrange
            var errorMessage = "error happened";

            _serviceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<User>>.Fail(errorMessage));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var returnedObj = Assert.IsType<ObjectResult>(result);
            var errorResponse = Assert.IsType<ApiErrorResponse>(returnedObj.Value);
            var returnedMessage = errorResponse.Message;

            Assert.Equal(errorMessage, returnedMessage);
        }

        [Fact]
        public async Task GetRolesAsync_WhenServiceSucceeds_ReturnsOkAndAListOfRoles()
        {
            // Arrange
            var testRoles = EnumExtensions.ToRoleDtoList(); // real source of roles

            _serviceMock
                .Setup(s => s.GetRolesAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<RoleDto>>.Ok(testRoles));

            // Act
            var result = await _controller.GetRolesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRoles = Assert.IsAssignableFrom<OkObjectResult>(okResult);
        }

        [Fact]
        public async Task GetRolesAsync_WhenServiceFails_ReturnsErrorMessage()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetRolesAsync())
                .ReturnsAsync(ServiceResult<IEnumerable<RoleDto>>.Fail("Error happend"));

            // Act
            var result = await _controller.GetRolesAsync();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var returnedRoles = Assert.IsAssignableFrom<ApiErrorResponse>(okResult.Value);
            Assert.Equal("Error happend", returnedRoles.Message);
        }

        [Fact]
        public async Task GetById_WhenServiceSucceeds_ReturnsOkWithUser()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Username = "test"
            };

            _serviceMock
                .Setup(s => s.GetByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.Ok(user));

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiSuccessResponse<User>>(okResult.Value);

            Assert.NotNull(response.Data);
            Assert.Equal(userId, response.Data.Id);
            Assert.Equal("test@example.com", response.Data.Email);
        }

        [Fact]
        public async Task GetById_WhenServiceFails_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;
            string errorMessage = "User not found";

            _serviceMock
                .Setup(s => s.GetByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.Fail(errorMessage));

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiErrorResponse>(notFoundResult.Value);

            Assert.Equal(errorMessage, response.Message);
        }

        [Fact]
        public async Task Create_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var newUser = new User { Email = "duplicate@example.com" };
            string errorMessage = "Email already registered";

            _serviceMock
                .Setup(s => s.CreateAsync(newUser))
                .ReturnsAsync(ServiceResult<User>.Fail(errorMessage));

            // Act
            var result = await _controller.Create(newUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var response = Assert.IsType<ApiErrorResponse>(badRequestResult.Value);
            Assert.Equal(errorMessage, response.Message);
        }

        [Fact]
        public async Task Create_WhenServiceSucceeds_ReturnsCreatedAtAction()
        {
            // Arrange
            var newUser = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser"
            };

            _serviceMock
                .Setup(s => s.CreateAsync(newUser))
                .ReturnsAsync(ServiceResult<User>.Ok(newUser));

            // Act
            var result = await _controller.Create(newUser);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(nameof(UserController.GetById), createdResult.ActionName);
            Assert.Equal(newUser.Id, createdResult.RouteValues["id"]);

            var response = Assert.IsType<ApiSuccessResponse<User>>(createdResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(newUser.Id, response.Data.Id);
            Assert.Equal(newUser.Email, response.Data.Email);
        }

        [Fact]
        public async Task Delete_WhenServiceSucceeds_ReturnsNoContent()
        {
            // Arrange
            int idOfUser = 1;
            _serviceMock
                .Setup(s => s.Delete(idOfUser))
                .ReturnsAsync(ServiceResult<bool>.Ok(true));

            // Act
            var result = await _controller.Delete(idOfUser);

            // Assert
            var createdResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenServiceFails_ReturnsNotFoundAndErrorMessage()
        {
            // Arrange
            int idOfUser = 1;
            _serviceMock
                .Setup(s => s.Delete(idOfUser))
                .ReturnsAsync(ServiceResult<bool>.Fail("Error occurred"));

            // Act
            var result = await _controller.Delete(idOfUser);

            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultContents = Assert.IsAssignableFrom<ApiErrorResponse>(createdResult.Value);
            Assert.Equal("Error occurred", resultContents.Message);
        }

        [Fact]
        public async Task Update_WhenServiceFails_ReturnsNotFoundAndErrorMessage()
        {
            // Arrange
            int idOfUser = 1;
            var updatedUser = new User { Email = "test@test.com" };
            _serviceMock
                .Setup(s => s.UpdateAsync(idOfUser, updatedUser))
                .ReturnsAsync(ServiceResult<bool>.Fail("Error occurred"));

            // Act
            var result = await _controller.Update(idOfUser, updatedUser);

            // Assert
            var createdResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultContents = Assert.IsAssignableFrom<ApiErrorResponse>(createdResult.Value);
            Assert.Equal("Error occurred", resultContents.Message);
        }

        [Fact]
        public async Task Update_WhenServiceSucceeds_ReturnsNoContent()
        {
            // Arrange
            int idOfUser = 1;
            var updatedUser = new User { Email = "test@test.com" };
            _serviceMock
                .Setup(s => s.UpdateAsync(idOfUser, updatedUser))
                .ReturnsAsync(ServiceResult<bool>.Ok(true));

            // Act
            var result = await _controller.Update(idOfUser, updatedUser);

            // Assert
            var createdResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePassword_WhenServiceSucceeds_ReturnsOkWithServiceResultObj()
        {
            // Arrange
            int idOfUser = 1;
            string currentPassword = "passwordTest";
            string newPassword = "passwordProtected123";

            _serviceMock
                .Setup(s => s.UpdatePasswordAsync(idOfUser, currentPassword, newPassword))
                .ReturnsAsync(ServiceResult<object>.Ok("Saved new password, you will now be redirected to login."));

            // Act
            var clientCollection = new Dictionary<string, string>
            {
                { "currentPassword", currentPassword },
                { "newPassword", newPassword }
            };

            var result = await _controller.UpdatePassword(idOfUser, clientCollection);

            // Assert
            var createdResult = Assert.IsType<OkObjectResult>(result);
            var resultContent = Assert.IsAssignableFrom<ServiceResult<object>>(createdResult.Value);
            Assert.Equal("Saved new password, you will now be redirected to login.", resultContent.Data);
        }

        [Fact]
        public async Task UpdatePassword_WhenServiceFails_ReturnsBadRequestAndMessage()
        {
            // Arrange
            int idOfUser = 1;
            string currentPassword = "passwordTest";
            string newPassword = "passwordProtected123";

            _serviceMock
                .Setup(s => s.UpdatePasswordAsync(idOfUser, currentPassword, newPassword))
                .ReturnsAsync(ServiceResult<object>.Fail("Error occurred"));

            // Act
            var clientCollection = new Dictionary<string, string>
            {
                { "currentPassword", currentPassword },
                { "newPassword", newPassword }
            };

            var result = await _controller.UpdatePassword(idOfUser, clientCollection);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var body = Assert.IsType<ApiErrorResponse>(badRequest.Value);

            Assert.Equal("Error occurred", body.Message);
        }
    }
}
