﻿namespace Aoxe.Server;

internal class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AoxeException ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
        catch (ArgumentNullException ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
        catch (ApplicationException ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, 600);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, int httpStatusCode)
    {
        var inmostEx = ex;
        while (inmostEx.InnerException is not null)
            inmostEx = inmostEx.InnerException;
        context.Response.StatusCode = httpStatusCode;
        var AoxeError = new AoxeError
        {
            Id = inmostEx is AoxeException AoxeException ? AoxeException.Id : Guid.NewGuid(),
            Message = inmostEx.Message,
            Source = inmostEx.Source,
            StackTrace = inmostEx.StackTrace,
            ThrowTime = DateTimeOffset.Now
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(AoxeError));
    }
}
