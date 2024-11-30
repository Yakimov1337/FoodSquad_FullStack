using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var errorResponse = new
        {
            error = "An error occurred",
            message = context.Exception.Message
        };

        context.Result = new JsonResult(errorResponse)
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
    }
}
