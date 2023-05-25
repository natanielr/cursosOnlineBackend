using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Aplicacion.HandlerError;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace WebAPI.middleware
{
    public class Error
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HandlerException> _logger;
        public Error(RequestDelegate next, ILogger<HandlerException> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) // este metodo se tiene que llamar asi Invoke 
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {

                await handleExceptionAsync(context, ex, _logger);
            }
        }

        private async Task handleExceptionAsync(HttpContext context, Exception exception, ILogger<HandlerException> _ILogger)
        {
            object errors = null;

            switch (exception)
            { // aqui se van a manejar los errores
                case HandlerException he:
                // muestra los errores que se generen por casos donde la informacion enviada es inválida
                    _ILogger.LogError(exception, "handleException");
                    errors = he.errors;
                    context.Response.StatusCode = (int)he.statusCode;
                break;
                case Exception e:
                // errores internos de c#
                    _ILogger.LogError(e, "Error de servidor");
                    errors = string.IsNullOrWhiteSpace(e.Message)? "Error interno de servidor":e.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                break;
            }

            context.Response.ContentType = "application/json"; // formato de respuesta de mensajes de error

            if(errors != null) {
                // armar el json de los errores para generar la respuesta
                var resultados = JsonConvert.SerializeObject(new {errors});
                // enviar la respuesta de los errores en el cliente
                await context.Response.WriteAsync(resultados);
            } 
        }
    }
}
