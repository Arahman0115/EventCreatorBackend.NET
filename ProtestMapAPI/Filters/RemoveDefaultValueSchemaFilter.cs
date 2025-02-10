// RemoveDefaultValueSchemaFilter.cs

using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace ProtestMapAPI.Filters // You can change this to a relevant namespace if needed
{
    public class RemoveDefaultValueSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var property in schema.Properties)
            {
                if (property.Value.Default != null)
                {
                    property.Value.Default = null; // Remove the default value
                }
            }
        }
    }
}
