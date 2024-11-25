namespace WidgetStore.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the request is invalid
    /// </summary>
    public class BadRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the BadRequestException class
        /// </summary>
        public BadRequestException() : base() { }

        /// <summary>
        /// Initializes a new instance of the BadRequestException class with a message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public BadRequestException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the BadRequestException class with a message and inner exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}