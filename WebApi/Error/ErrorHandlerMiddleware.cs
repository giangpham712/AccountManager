using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Common.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AccountManager.WebApi.Error
{
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                ProblemDetails responseModel;
                _logger.LogError(exception.Message, exception);

                switch (exception)
                {
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        var validationErrorsByProperty = e.Errors
                                                          .GroupBy(x => x.PropertyName)
                                                          .ToDictionary(x => x.Key.ToCamelCase(),
                                                                        x => x.Select(y => y.ErrorMessage)
                                                                              .ToArray());

                        responseModel = new ValidationProblemDetails(validationErrorsByProperty);

                        break;
                    case EntityNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel = new ProblemDetails();

                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel = new ProblemDetails();

                        break;
                }

                responseModel.Title = exception.Message;
                responseModel.Detail = exception.Message;
                responseModel.Status = response.StatusCode;
                var result = JsonSerializer.Serialize(responseModel, responseModel.GetType());
                await response.WriteAsync(result);
            }
        }
    }
}
