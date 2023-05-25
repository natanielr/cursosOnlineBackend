using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.HandlerError
{
    public class HandlerException : Exception // para manejar cualquier tipo de excepcio, debe heredar los recursos de la clase Excepcion
    {

        public HttpStatusCode statusCode{get;}
        public object errors{get;}
        public HandlerException(HttpStatusCode statusCode, object errors = null)
        {
            this.statusCode = statusCode;
            this.errors = errors;
        }
    }
}