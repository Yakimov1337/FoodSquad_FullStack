using FoodSquad_API.Models.DTO;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody?.Content?.ContainsKey("application/json") ?? false)
        {
            var jsonContent = operation.RequestBody.Content["application/json"];
            var schema = jsonContent.Schema;

            if (context.ApiDescription.ParameterDescriptions.Any(p => p.ModelMetadata?.ModelType == typeof(UserLoginDTO)))
            {
                jsonContent.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["email"] = new Microsoft.OpenApi.Any.OpenApiString("admin@example.com"),
                    ["password"] = new Microsoft.OpenApi.Any.OpenApiString("123123")
                };
            }
        }
    }
}
