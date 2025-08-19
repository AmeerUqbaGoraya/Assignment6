// Author: Your Name
// Date: August 19, 2025

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Assignment6.Models.DTOs;

namespace Assignment6.Infrastructure.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var response = new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No implementation needed
        }
    }
}
