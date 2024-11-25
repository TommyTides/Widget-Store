namespace WidgetStore.Core.Entities
{
    /// <summary>
    /// Base entity class that provides common properties for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Timestamp when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the entity was last modified
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
    }
}