using FluentAssertions;
using WidgetStore.Shared.Helpers;
using Xunit;

namespace WidgetStore.UnitTests.Helpers
{
    /// <summary>
    /// Tests for the HashingHelper class
    /// </summary>
    public class HashingHelperTests
    {
        [Fact(DisplayName = "HashPassword should create different hashes for different passwords")]
        public void HashPassword_ShouldCreateDifferentHashesForDifferentPasswords()
        {
            // Arrange
            var password1 = "Password123!";
            var password2 = "Password123@";

            // Act
            var hash1 = HashingHelper.HashPassword(password1);
            var hash2 = HashingHelper.HashPassword(password2);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact(DisplayName = "HashPassword should create same hash for same password")]
        public void HashPassword_ShouldCreateSameHashForSamePassword()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash1 = HashingHelper.HashPassword(password);
            var hash2 = HashingHelper.HashPassword(password);

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact(DisplayName = "VerifyPassword should return true for correct password")]
        public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
        {
            // Arrange
            var password = "Password123!";
            var hash = HashingHelper.HashPassword(password);

            // Act
            var result = HashingHelper.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact(DisplayName = "VerifyPassword should return false for incorrect password")]
        public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
        {
            // Arrange
            var password = "Password123!";
            var wrongPassword = "Password123@";
            var hash = HashingHelper.HashPassword(password);

            // Act
            var result = HashingHelper.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }
    }
}