using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.API.Filters
{
    public class ValidationFilter<T> : IAsyncActionFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var arg = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (arg is not null)
            {
                var result = await _validator.ValidateAsync(arg);

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Validation failed.",
                        Errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    });
                    return;
                }
            }

            await next();
        }
    }
}