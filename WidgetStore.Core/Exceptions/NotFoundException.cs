namespace WidgetStore.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NotFoundException class
        /// </summary>
        public NotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class with a message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the NotFoundException class with a message and inner exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that caused this exception</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}