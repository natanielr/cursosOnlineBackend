using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Aplicacion.Comentarios;

namespace WebAPI.Controllers
{
    public class ComentarioController : MiControllerBase
    {
        [HttpPost("nuevo")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<Unit>> Nuevo(NuevoComentario.NuevoComentarioData data)
        {
            return await mediator.Send(data);
        }

        [HttpDelete("{id}")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await mediator.Send(new EliminarComentario.EliminarComentarioData { Id = id });
        }
    }
}