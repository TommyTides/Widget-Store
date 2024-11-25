using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using WidgetStore.Core.Entities;
using WidgetStore.Infrastructure.Entities;
using WidgetStore.Infrastructure.Repositories;
using WidgetStore.Shared.Constants;
using WidgetStore.UnitTests.Helpers;
using Xunit;

namespace WidgetStore.UnitTests.Repositories
{
    /// <summary>
    /// Tests for the UserRepository class
    /// </summary>
    public class UserRepositoryTests
    {
        private readonly Mock<TableClient> _mockTableClient;
        private readonly UserRepository _userRepository;

        /// <summary>
        /// Initializes test context for UserRepository tests
        /// </summary>
        public UserRepositoryTests()
        {
            _mockTableClient = new Mock<TableClient>();
            _userRepository = new UserRepository(_mockTableClient.Object);
        }

        #region CreateAsync Tests

        [Fact(DisplayName = "CreateAsync should add entity successfully")]
        public async Task CreateAsync_ShouldAddEntity_Successfully()
        {
            // Arrange
            var user = TestData.GetTestUser();

            _mockTableClient.Setup(x => x.AddEntityAsync(
                It.IsAny<UserTableEntity>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Response>());

            // Act
            var result = await _userRepository.CreateAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);

            _mockTableClient.Verify(x => x.AddEntityAsync(
                It.Is<UserTableEntity>(e =>
                    e.PartitionKey == StorageConstants.UserPartitionKey &&
                    e.Email == user.Email),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "CreateAsync should throw when TableClient throws")]
        public async Task CreateAsync_ShouldThrow_WhenTableClientThrows()
        {
            // Arrange
            var user = TestData.GetTestUser();
            _mockTableClient.Setup(x => x.AddEntityAsync(
                It.IsAny<UserTableEntity>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Test error"));

            // Act
            var act = () => _userRepository.CreateAsync(user);

            // Assert
            await act.Should().ThrowAsync<RequestFailedException>();
        }

        #endregion

        #region GetByEmailAsync Tests

        [Fact(DisplayName = "GetByEmailAsync should return user when found")]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenFound()
        {
            // Arrange
            var email = "test@example.com";
            var testEntity = TableEntityHelper.CreateTestUserTableEntity();

            var asyncPageable = GetMockAsyncPageable(new[] { testEntity });
            _mockTableClient.Setup(x => x.QueryAsync<UserTableEntity>(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            // Act
            var result = await _userRepository.GetByEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
        }

        [Fact(DisplayName = "GetByEmailAsync should return null when not found")]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var email = "notfound@example.com";
            var asyncPageable = GetMockAsyncPageable(Array.Empty<UserTableEntity>());

            _mockTableClient.Setup(x => x.QueryAsync<UserTableEntity>(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
                .Returns(asyncPageable);

            // Act
            var result = await _userRepository.GetByEmailAsync(email);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact(DisplayName = "GetByIdAsync should return user when found")]
        public async Task GetByIdAsync_ShouldReturnUser_WhenFound()
        {
            // Arrange
            var id = "testId";
            var testEntity = TableEntityHelper.CreateTestUserTableEntity(id);

            _mockTableClient.Setup(x => x.GetEntityAsync<UserTableEntity>(
                StorageConstants.UserPartitionKey,
                id,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(testEntity, Mock.Of<Response>()));

            // Act
            var result = await _userRepository.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact(DisplayName = "GetByIdAsync should return null when not found")]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = "notFoundId";

            _mockTableClient.Setup(x => x.GetEntityAsync<UserTableEntity>(
                StorageConstants.UserPartitionKey,
                id,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException((int)System.Net.HttpStatusCode.NotFound, "Not Found"));

            // Act
            var result = await _userRepository.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact(DisplayName = "GetByIdAsync should throw on unexpected error")]
        public async Task GetByIdAsync_ShouldThrow_OnUnexpectedError()
        {
            // Arrange
            var id = "testId";

            _mockTableClient.Setup(x => x.GetEntityAsync<UserTableEntity>(
                StorageConstants.UserPartitionKey,
                id,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(500, "Internal Server Error"));

            // Act
            var act = () => _userRepository.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<RequestFailedException>();
        }

        #endregion

        #region Helper Methods

        private static AsyncPageable<T> GetMockAsyncPageable<T>(IEnumerable<T> items)
        {
            var page = Page<T>.FromValues(items.ToList(), null, Mock.Of<Response>());
            var pages = new[] { page };
            return AsyncPageable<T>.FromPages(pages);
        }

        #endregion
    }
}