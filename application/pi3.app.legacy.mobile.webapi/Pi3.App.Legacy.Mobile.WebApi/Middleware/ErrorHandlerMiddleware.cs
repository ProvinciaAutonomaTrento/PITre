// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc;
using Pi3.Core.SeedWork;
using System.IO.Pipelines;
using System.Net;
using System.Resources;
using System.Text.Json;

namespace Pi3.App.Legacy.Mobile.WebApi.Middleware;

public class ErrorHandlerMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlerMiddleware> logger)
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync( HttpContext context )
    {
        try
        {
            await _next(context);
        }
        catch ( Pi3Exception ex )
        {
            await this.HandlePi3ExceptionAsync(context, ex);
        }
        catch ( Exception ex )
        {
            await this.HandleExceptionAsync(context, ex);
        }
    }

    private Task HandlePi3ExceptionAsync( HttpContext context, Pi3Exception exception )
    {
        this._logger.LogError(exception, "{Message}", exception.Message);

        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
        context.Response.StatusCode = ((int)exception.StatusCode);

        var problemDetails = new ProblemDetails
        {
            Status = (int?)exception.StatusCode,
            Title = "An error occurred while processing your request.",
            Detail = exception.Message
        };





        problemDetails.Extensions["ErrorCode"] = exception.ErrorCode;
        problemDetails.Extensions["Message"] = exception.Message;
        problemDetails.Extensions["Type"] = exception.GetType().FullName;
        problemDetails.Extensions["StackTrace"] = null; // exception.StackTrace; // Lo escludo per motivi di sicurezza

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private Task HandleExceptionAsync( HttpContext context, Exception exception )
    {
        this._logger.LogError(exception, "{Message}", exception.Message);

        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.ProblemJson;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = exception.Message
        };
        problemDetails.Extensions["ErrorCode"] = "Errore non previsto";
        problemDetails.Extensions["Message"] = exception.Message;
        problemDetails.Extensions["Type"] = exception.GetType().FullName;
        problemDetails.Extensions["StackTrace"] = null; // exception.StackTrace; // Lo escludo per motivi di sicurezza

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

}
