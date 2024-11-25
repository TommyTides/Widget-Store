using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WidgetStore.Core.DTOs.Product
{
    /// <summary>
    /// DTO for product image upload
    /// </summary>
    public class ProductImageUploadDto
    {
        /// <summary>
        /// The image file to upload
        /// </summary>
        [Required]
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}