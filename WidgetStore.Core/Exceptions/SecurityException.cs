namespace WidgetStore.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when security-related operations fail
    /// </summary>
    public class SecurityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SecurityException class
        /// </summary>
        public SecurityException() : base() { }

        /// <summary>
        /// Initializes a new instance of the SecurityException class with a message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public SecurityException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the SecurityException class with a message and inner exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public SecurityException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}