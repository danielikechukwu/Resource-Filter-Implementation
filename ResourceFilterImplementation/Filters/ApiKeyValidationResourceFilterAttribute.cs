using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ResourceFilterImplementation.Filters
{
    public class ApiKeyValidationResourceFilterAttribute : Attribute, IResourceFilter
    {
        // Name of the HTTP header expected to contain the API key
        private const string ApiKeyHeaderName = "X-API-KEY";

        // This method runs before the action method executes
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // Access Header collection to check for the API key
            var headers = context.HttpContext.Request.Headers;

            if(!headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                // Prepare a JSON payload indicating unauthorized access due to missing API key
                var payload = new
                {
                    Status = 401,
                    Message = "Unauthorized: API Key is missing"
                };

                // Set the HTTP response immediately to 401 Unauthorized with the payload
                // This short-circuits the request pipeline and prevents the action from executing
                context.Result = new JsonResult(payload)
                {
                    StatusCode = StatusCodes.Status401Unauthorized // HTTP 401 Unauthorized
                };

                return; // Exit early since the API key is missing
            }

            // If API key header is present, no action is taken here and the request proceeds
        }

        // This method runs after the action method executes
        // No post-processing is needed in this filter for after the action execution
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // No implementation needed here
        }
    }
}
