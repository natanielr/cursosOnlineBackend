using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Aplicacion.Seguridad;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Controllers
{
    public class RoleController : MiControllerBase
    {
        [HttpPost("nuevo")]

        public async Task<ActionResult<Unit>> registrar(NuevoRole.NuevoRoleData data)
        {
            return await mediator.Send(data);
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult<Unit>> eliminar(string id)
        {
            return await mediator.Send(new EliminarRole.EliminarRoleData { Id = id });
        }

        [HttpGet]
        public async Task<ActionResult<List<IdentityRole>>> lista()
        {
            return await mediator.Send(new ListaRoles.ListaRolesData());
        }
        [HttpPost("usuario-role")]

        public async Task<ActionResult<Unit>> usuarioRole(UsuarioRole.UsuarioRoleData data)
        {
            return await mediator.Send(data);
        }

        [HttpDelete("usuario-role/{userId}/{roleId}")]

        public async Task<ActionResult<Unit>> usuarioRoleEliminar(string userId, string roleId)
        {

            var data = new UsuarioRoleEliminar.UsuarioRoleEliminarData
            {
                RoleId = roleId,
                UserId = userId
            };
            return await mediator.Send(data);
        }

        [HttpGet("usuario-role/{userId}")]

        public async Task<List<string>> rolesPorUsuario(string userId)
        {
            return await mediator.Send(new RolesPorUsuario.RolesPorUsuarioData
            {
                UserId = userId
            });
        }
    }
}