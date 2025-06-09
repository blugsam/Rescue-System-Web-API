using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RescueSystem.Api.Exceptions;
using RescueSystem.Application.Exceptions;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;
                if (exception == null) return;

                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(exception, "Unhandled exception");

                (int code, ProblemDetails details) = exception switch
                {
                    NotFoundException nf => (404, new ProblemDetails
                    {
                        Status = 404,
                        Title = "Resource Not Found",
                        Detail = nf.Message
                    }),
                    BadRequestException br => (400, new ValidationProblemDetails(br.Errors ?? new Dictionary<string, string[]>())
                    {
                        Status = 400,
                        Title = "Bad Request",
                        Detail = br.Message
                    }),
                    _ => (500, new ProblemDetails
                    {
                        Status = 500,
                        Title = "Internal Server Error",
                        Detail = context.RequestServices
                                       .GetRequiredService<IHostEnvironment>()
                                       .IsDevelopment()
                                   ? exception.ToString()
                                   : "Please contact support."
                    })
                };

                context.Response.StatusCode = code;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(details);
            });
        });

        return app;
    }
}
