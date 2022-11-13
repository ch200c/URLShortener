using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using URLShortener.Application;

namespace URLShortener.API.SchemaFilters;

public class CreateShortenedEntryRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateShortenedEntryRequest))
        {
            var aliasSchema = schema.Properties["alias"];
            aliasSchema.Type = "string";
            aliasSchema.Items = null;
            aliasSchema.Nullable = true;

            schema.Example = new OpenApiObject
            {
                { "alias", new OpenApiString("string") },
                { "url", new OpenApiString("string") },
                { "expiration", new OpenApiDateTime(DateTimeOffset.UtcNow) }
            };
        }
    }
}
