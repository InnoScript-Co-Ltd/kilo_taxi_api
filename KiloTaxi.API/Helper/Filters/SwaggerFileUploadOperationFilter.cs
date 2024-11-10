using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace KiloTaxi.API.Helper.Filters
{
    public class SwaggerFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //// Skip adding request body for GET methods
            //if (operation.OperationId != null && context.MethodInfo.GetCustomAttributes()
            //    .Any(attr => attr.GetType().Name == "HttpGetAttribute"))
            //{
            //    return;
            //}
            // Check if the method has the HttpGetAttribute
            if (context.MethodInfo.GetCustomAttribute<HttpGetAttribute>() != null)
            {
                return; // Skip adding a request body for GET methods
            }

            // Check if the method is a POST or PUT
            if (context.MethodInfo.GetCustomAttribute<HttpPostAttribute>() != null ||
                context.MethodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
            {
                // Keep only route parameters for PUT methods
                if (context.MethodInfo.GetCustomAttribute<HttpPutAttribute>() != null)
                {
                    operation.Parameters = operation.Parameters
                        .Where(p => p.In == ParameterLocation.Path).ToList();
                }
                else
                {
                    // Remove parameters to avoid showing them along with the request body for POST methods
                    operation.Parameters.Clear();
                }
            }

            // Process and include request body for complex types (e.g., DTOs)
            // Get the parameters of the operation's action
            var actionParameters = context.MethodInfo.GetParameters();

            foreach (var parameter in actionParameters)
            {
                // Check if the parameter is a complex type
                if (parameter.ParameterType.IsClass && parameter.ParameterType != typeof(string))
                {
                    // Get all properties of the parameter type
                    var properties = parameter.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    // Ensure operation has a request body
                    operation.RequestBody ??= new OpenApiRequestBody();

                    var formDataSchema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = properties.ToDictionary(
                            prop => prop.Name,
                            prop => new OpenApiSchema
                            {
                                //Type = prop.PropertyType == typeof(IFormFile) ? "string" : "string",
                                Type = GetOpenApiType(prop.PropertyType),
                                Format = prop.PropertyType == typeof(IFormFile) ? "binary" : null, // Indicates file upload
                                Description = prop.Name, // Set the column name as a placeholder
                                Example = new OpenApiString(string.Empty),
                                Default = null // Ensure no default value is set
                            })
                    };

                    operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = formDataSchema
                    };
                }
            }
        }
        private string GetOpenApiType(Type type)
        {
            return type == typeof(int) ? "integer" :
                   type == typeof(long) ? "integer" :
                   type == typeof(float) || type == typeof(double) || type == typeof(decimal) ? "number" :
                   type == typeof(bool) ? "boolean" :
                   type == typeof(IFormFile) ? "string" :
                   "string"; // Default to string for other types
        }

    }
}
