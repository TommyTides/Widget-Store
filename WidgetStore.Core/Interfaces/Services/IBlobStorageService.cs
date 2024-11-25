namespace WidgetStore.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for blob storage operations
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads a file to blob storage
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contentType">Content type of the file</param>
        /// <param name="stream">File stream</param>
        /// <returns>URL of the uploaded blob</returns>
        Task<string> UploadAsync(string containerName, string fileName, string contentType, Stream stream);

        /// <summary>
        /// Deletes a file from blob storage
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="fileName">Name of the file to delete</param>
        Task DeleteAsync(string containerName, string fileName);

        /// <summary>
        /// Gets a file from blob storage
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>File stream</returns>
        Task<Stream> GetAsync(string containerName, string fileName);

        /// <summary>
        /// Checks if a file exists in blob storage
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>True if file exists, false otherwise</returns>
        Task<bool> ExistsAsync(string containerName, string fileName);
    }
}