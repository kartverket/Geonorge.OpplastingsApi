using Azure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Utilities;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Geonorge.OpplastingsApi.Middleware
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext
         context)
        {
            var formParameters = context.ApiDescription.ParameterDescriptions
                .Where(paramDesc => paramDesc.IsFromForm());

            if (formParameters.Any())
            {
                // already taken care by swashbuckle. no need to add explicitly.
                //return;
            }
            if (operation.RequestBody != null)
            {
                // NOT required for form type
                //return;
            }
            if (context.ApiDescription.HttpMethod == HttpMethod.Post.Method && context.ApiDescription.RelativePath == "Dataset/file")
            {
                var uploadFileMediaType = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = "object",
                        Properties =
                    {
                        ["files"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type="file",
                                Format="binary"
                            }
                        },
                        ["datasetId"] = new OpenApiSchema()
                        {
                            Type = "int"
                        }
                    },
                        Required = new HashSet<string>() { "files" },
                    }
                };

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = { ["multipart/form-data"] = uploadFileMediaType }
                };
            }

        }
    }

    public static class Helper
    {
        internal static bool IsFromForm(this ApiParameterDescription
        apiParameter)
        {
            var source = apiParameter.Source;
            var elementType = apiParameter.ModelMetadata?.ElementType;

            return (source == BindingSource.Form || source ==
            BindingSource.FormFile) || (elementType != null &&
            typeof(IFormFile).IsAssignableFrom(elementType));
        }
    }
}