using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace JB.Lib.Extensions.Filters
{
    public class ModelValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Keys.Zip(context.ModelState.Values, (k, v) => new { Field = k, v.Errors });

                var errorField = errors?.FirstOrDefault()?.Field;
                var errorMessage = errors?.FirstOrDefault()?.Errors?.FirstOrDefault()?.ErrorMessage;

                context.Result = new BadRequestObjectResult(new
                {
                    message = errorMessage,
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
