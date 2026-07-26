using Microsoft.AspNetCore.Mvc;

namespace TravelManagement.API.Infrastructure.Validation;

public static class ValidationResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            StatusCode = 400,
            Message = "Validation failed.",
            Errors = errors
        });
    }
}