﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using Hurudza.Apis.Base.Models;
using Hurudza.Common.Utils.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hurudza.Apis.Base.Infrastructure.Filters;

public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment env;
    private readonly ILogger<HttpGlobalExceptionFilter> logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    {
        this.env = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        if (context.Exception is ValidationException)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Result = new JsonResult(
                ((ValidationException)context.Exception).Data);
            return;
        }
        else if (context.Exception.GetType() == typeof(CustomException))
        {
            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details."
            };

            problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            HttpStatusCode statusCode = (context.Exception as WebException != null &&
                                         ((HttpWebResponse)(context.Exception as WebException).Response) != null) ?
                ((HttpWebResponse)(context.Exception as WebException).Response).StatusCode
                : getErrorCode(context.Exception.GetType());

            var json = new JsonErrorResponse
            {
                Messages = new[] { "An error ocurred." }
            };

            if (env.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;

            context.Result = new JsonResult(new ApiResponse((int)statusCode, context.Exception.Message, context.Exception.StackTrace));
        }

        context.ExceptionHandled = true;
    }

    private class JsonErrorResponse
    {
        public string[] Messages { get; set; }

        public object DeveloperMessage { get; set; }
    }

    /// <summary>  
    /// This method will return the status code based on the exception type.  
    /// </summary>  
    /// <param name="exceptionType"></param>  
    /// <returns>HttpStatusCode</returns>  
    private HttpStatusCode getErrorCode(Type exceptionType)
    {
        Exceptions tryParseResult;
        if (Enum.TryParse(exceptionType.Name, out tryParseResult))
        {
            switch (tryParseResult)
            {
                case Exceptions.NullReferenceException:
                    return HttpStatusCode.LengthRequired;

                case Exceptions.FileNotFoundException:
                    return HttpStatusCode.NotFound;

                case Exceptions.NotFoundException:
                    return HttpStatusCode.NotFound;

                case Exceptions.OverflowException:
                    return HttpStatusCode.RequestedRangeNotSatisfiable;

                case Exceptions.OutOfMemoryException:
                    return HttpStatusCode.ExpectationFailed;

                case Exceptions.InvalidCastException:
                    return HttpStatusCode.PreconditionFailed;

                case Exceptions.ObjectDisposedException:
                    return HttpStatusCode.Gone;

                case Exceptions.UnauthorizedAccessException:
                    return HttpStatusCode.Unauthorized;

                case Exceptions.NotImplementedException:
                    return HttpStatusCode.NotImplemented;

                case Exceptions.NotSupportedException:
                    return HttpStatusCode.NotAcceptable;

                case Exceptions.InvalidOperationException:
                    return HttpStatusCode.MethodNotAllowed;

                case Exceptions.TimeoutException:
                    return HttpStatusCode.RequestTimeout;

                case Exceptions.ArgumentException:
                    return HttpStatusCode.BadRequest;

                case Exceptions.StackOverflowException:
                    return HttpStatusCode.RequestedRangeNotSatisfiable;

                case Exceptions.FormatException:
                    return HttpStatusCode.UnsupportedMediaType;

                case Exceptions.IOException:
                    return HttpStatusCode.NotFound;

                case Exceptions.IndexOutOfRangeException:
                    return HttpStatusCode.ExpectationFailed;

                default:
                    return HttpStatusCode.InternalServerError;
            }
        }

        return HttpStatusCode.InternalServerError;
    }
}