using FluentAssertions;
using WidgetStore.Shared.Helpers;
using Xunit;

namespace WidgetStore.UnitTests.Helpers
{
    /// <summary>
    /// Tests for the ValidationHelper class
    /// </summary>
    public class ValidationHelperTests
    {
        [Theory(DisplayName = "IsValidEmail should correctly validate email addresses")]
        [InlineData("test@example.com", true)]
        [InlineData("test.name@example.com", true)]
        [InlineData("test+name@example.com", true)]
        [InlineData("invalid.email", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("test@", false)]
        [InlineData("@example.com", false)]
        public void IsValidEmail_ShouldValidateEmailCorrectly(string email, bool expected)
        {
            // Act
            var result = ValidationHelper.IsValidEmail(email);

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "IsValidPassword should correctly validate passwords")]
        [InlineData("Password123!", true)]
        [InlineData("sh0rt", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("verylongpasswordthatexceedsmaximumlengthverylongpasswordthatexceedsmaximumlengthverylongpasswordthatexceedsmaximumlength", false)]
        public void IsValidPassword_ShouldValidatePasswordCorrectly(string password, bool expected)
        {
            // Act
            var result = ValidationHelper.IsValidPassword(password);

            // Assert
            result.Should().Be(expected);
        }
    }
}