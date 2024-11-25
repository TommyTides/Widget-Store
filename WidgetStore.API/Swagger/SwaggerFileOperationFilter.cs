using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WidgetStore.API.Swagger
{
    /// <summary>
    /// Filter to handle file upload operations in Swagger
    /// </summary>
    public class SwaggerFileOperationFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies the filter to handle file upload operations
        /// </summary>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(IFormFile))
            {
                schema.Type = "string";
                schema.Format = "binary";
            }
        }
    }
}