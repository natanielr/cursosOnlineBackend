using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Microsoft.AspNetCore.Http;
namespace Seguridad.TokenSeguridad
{
    public class UsuarioSesion : IUsuarioSesion
    {   
        private readonly IHttpContextAccessor contextAccesor;
        public UsuarioSesion(IHttpContextAccessor _contextAccesor) {
            this.contextAccesor = _contextAccesor;
        }
        public string ObtenerUsuarioActual()
        {

           // obtener el usaurio actual de la sesion
           var userName = contextAccesor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
           return userName;
        }
    }
}