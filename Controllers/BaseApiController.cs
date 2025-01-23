using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroServices.Shared.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        private ILogger<T> _loggerInstance;
        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IWebHostEnvironment _env => HttpContext.RequestServices.GetService<IWebHostEnvironment>();


        protected Result<T> BuildValidationResponse(Result<T> obj)
        {
            var errors = this.GetModelStateValidationErrorsAsList();
            obj.Errors = errors.Any() ? errors : null;
            obj.Message = errors.Any() ? errors.FirstOrDefault() : null;

            return obj;
        }

        protected List<string> GetModelStateValidationErrorsAsList()
        {
            var message = ModelState.Values.SelectMany(a => a.Errors).Select(e => e.ErrorMessage);
            var list = new List<string>();
            list.AddRange(message);
            return list;
        }

        protected string GetModelStateValidationErrors()
        {
            string message = string.Join(";", ModelState.Values
                                    .SelectMany(a => a.Errors)
                                    .Select(e => e.ErrorMessage));
            return message;
        }

        protected string GetModelStateValidationError()
        {
            string message = ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
            return message;
        }

        protected IActionResult HandleError(Exception ex, string customErrorMessage = null)
        {
            Result rsp = new();
            bool isDev = _env.IsDevelopment();
            string errorMsg = "An error occurred while processing your request!";

            if (isDev)
            {
                if (!string.IsNullOrEmpty(ex?.InnerException?.Message))
                {
                    errorMsg = ex?.InnerException?.Message;
                }
                else if (!string.IsNullOrEmpty(ex?.Message))
                {
                    errorMsg = ex?.Message;
                }
            }

            _logger.LogError(ex, customErrorMessage ?? errorMsg);

            if (isDev)
            {
                rsp.Message = $"{(errorMsg)} --> {ex?.StackTrace}";
            }
            else
            {
                rsp.Message = customErrorMessage ?? errorMsg;
            }

            return StatusCode(StatusCodes.Status500InternalServerError, rsp);
        }
    }
}