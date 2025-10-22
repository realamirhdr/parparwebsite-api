using Microsoft.AspNetCore.Diagnostics;

namespace ParParWebsite.Api.Middleware
{
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this WebApplication? app)
        {
            if (app.Environment.IsDevelopment())
            {
                // In dev, show detailed exception page
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // In prod, catch exceptions and return JSON
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = context.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                        var feature = context.Features.Get<IExceptionHandlerFeature>();
                        var ex = feature?.Error;

                        // 1) Log the exception
                        logger.LogError(ex, "Unhandled exception");

                        // 2) Choose status code
                        //var status = ex is BusinessException ? 400 : 500;
                        var status = 500;

                        // 3) Return JSON payload
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = status;

                        var payload = new
                        {
                            error = ex?.Message,

#if DEBUG
                            trace = ex.StackTrace
#endif
                        };

                        await context.Response
                            .WriteAsJsonAsync(payload, cancellationToken: default);
                    });
                });
            }


        }
    }
}
