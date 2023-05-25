using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [AllowAnonymous] // permitir qye este servicio se pueda consumir sin necesidad de envio de token
    public class UsuarioController: MiControllerBase
    {
        [HttpPost("login")] // esta ruta va extender junto con la ruta base de MiControllerBase
        // la ruta seria http://localhost:5000/api/Usuario/login

        // toma la ruta base que es api, la que esta asignada en MiControllerBase.
        // Toma como parte de la ruta el nombre del controlador que en este caso es Usuario, y luego el nombre del URI que fue asignado 
        // como login
        public async Task<ActionResult<UsuarioResponse>> login (Login.LoginData data) {

            return await mediator.Send(data);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioResponse>> registrar (Registrar.RegistrarData data) {
            return await mediator.Send(data);
        } 

        [HttpGet("usuario-actual")]
        public async Task<ActionResult<UsuarioResponse>> getUsuario() {
            return await mediator.Send(new UsuarioActual.UsuarioActualData() ); // cuando la clase que tiene los parametros de entrada del request
            // no tiene propiedades
        }

        [HttpPut]
        public async Task<ActionResult<UsuarioResponse>> actualizar(ActualizarUsuario.ActualizarUsuarioData data){
            return await mediator.Send(data);
        }
    }
}