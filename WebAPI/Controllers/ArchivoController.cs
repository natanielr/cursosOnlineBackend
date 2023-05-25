using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Documentos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers
{
    public class ArchivoController: MiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Unit>> subirArchivo(SubirArchivo.SubirArchivoData data) { 
            return await mediator.Send(data);
        }

        [HttpGet("{usuarioId}")]
        public async Task<ActionResult<ArchivoGenerico>> ConsultarArchivo(Guid usuarioId) { 
            return await mediator.Send(new ObtenerArchivo.ObtenerArchivoData {
                UsuarioId = usuarioId
            });
        }
    }
}