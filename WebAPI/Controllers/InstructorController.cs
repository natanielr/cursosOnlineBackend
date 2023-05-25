using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Instructores;
using Aplicacion.Instructores;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    
    public class InstructorController: MiControllerBase
    {
        //administrar rutas por roles
        [Authorize(Roles = "Admin,SuperAdmin")] // autorizar por varios roles
        [HttpGet]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<List<InstructorModel>>> Get() {
            return await mediator.Send(new Consulta.ListaInstructores());
        }   

        [HttpPost("nuevo")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<Unit>> Nuevo(NuevoInstructor.NuevoInstructorData data) {
            return await mediator.Send(data);
        }  

        [HttpGet("{id}")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<InstructorModel>> GetById(Guid id) {
            return await mediator.Send(new ConsultaId.InstructorId{Id = id});
        } 

    // actualizar 
        [HttpPut("{id}")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<Unit>> actualizar(EditarInstructor.EditarInstructorData data, Guid id) {
            data.InstructorId = id;
            return await mediator.Send(data);
        } 

         [HttpDelete("{id}")]
        //[Authorize] // para validar que el token se envie por la cabecera
        public async Task<ActionResult<Unit>> Eliminar( Guid id) {
            return await mediator.Send(new EliminarInsstructor.EliminarInstructorData{Id = id});
        } 
    }
}