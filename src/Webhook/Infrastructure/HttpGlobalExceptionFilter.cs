using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Webhook.Configuration;

namespace Webhook.Infrastructure
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<HttpGlobalExceptionFilter> iLogger;

        public HttpGlobalExceptionFilter(IOptions<AppSettings> appSettings, ILogger<HttpGlobalExceptionFilter> iLogger)
        {
            this.appSettings = appSettings.Value;
            this.iLogger = iLogger;
        }

        public void OnException(ExceptionContext context)
        {
            int code = StatusCodes.Status500InternalServerError;

            double? errorCode = default;

            switch (context.Exception)
            {
                #region Status Code selon les exceptions

                #endregion
            }

            context.Result = new ObjectResult(new ErrorResult(context.Exception.Message, errorCode,context.Exception.StackTrace));
            context.HttpContext.Response.StatusCode = code;

            context.ExceptionHandled = true;
        }
    }
}
