using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Dominio;
using Aplicacion.Cursos; // agregar la referencia del proyecto para consumir los recursos
using System;
using Microsoft.AspNetCore.Authorization;
using Aplicacion.Dto;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI.Controllers
{

    public class CursosController : MiControllerBase
    {

        [HttpGet]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<List<CursoDto>>> Get()
        {
            return await mediator.Send(new Consulta.ListaCursos());
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<CursoDto>> GetById(Guid id)
        {
            return await mediator.Send(new ConsultaId.CursoId { Id = id }); // aqui se setea el parametro id para buscar el curso
        }
        [HttpPost]
        public async Task<ActionResult<Unit>> Nuevo(NuevoCurso.NuevoCursoData data)
        {

            return await mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, EditarCurso.EditarCursoData data)
        {
            // el primer parametro es el enviado por la url o sea el id del curso
            data.CursoId = id; // luego se setea en los datos de entrada para actualizar el curso
            return await mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await mediator.Send(new EliminarCurso.EliminarCursoData { Id = id });
        }

        [HttpPost("report")]
        public async Task<ActionResult<PaginacionModel>> obtenerCursosPaginados(PaginacionCurso.PaginacionCursoData data)
        {
            return await mediator.Send(data);
        }
    }
}