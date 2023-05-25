using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class EditarCurso
    {
        public class EditarCursoData : IRequest
        {

            public Guid CursoId { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }
            // public byte[] FotoPortada {get;set;}

            public List<Guid> ListaInstructor { get; set; }

            public decimal Precio { get; set; }
            public decimal Promocion { get; set; }
        }

        public class EditarCursoDataValidaciones : AbstractValidator<EditarCursoData>
        {
            public EditarCursoDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }


        public class Handler : IRequestHandler<EditarCursoData>
        {
            private readonly CursosOnLineContext context;

            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }
            public async Task<Unit> Handle(EditarCursoData request, CancellationToken cancellationToken)
            {
                // CancellationToken cancellationToken es un parametro que cancela una consulta a nivel de servidor cuando un servicio
                // dura mucho en responder y el cliente cancela el request desde el navegador
                var curso = await this.context.Curso.FindAsync(request.CursoId);

                if (curso == null) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "No se encontro el curso"
                }); //throw new Exception("Curso no encontrado");


                // aqui se actualizan los campos del curso
                curso.Descripcion = request.Descripcion ?? curso.Descripcion; //?? valor que evalua si el valor de una variable es nulo, indefinido o vacio.
                // en tal caso, dejaria el titulo actual sino actualizaria el titulo con el que viene el request
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;

                //tiene que venir una lista de instructorescon al menos un instructor para actualizar
                if (request.ListaInstructor?.Count > 0)
                {

                    // borrar los instructores actuales 

                    var instructoresCurso = this.context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();

                    // agregar los nuevos instructores

                    if (instructoresCurso?.Count > 0)
                    {
                        // aqui se borran los instructores

                        foreach (var id in instructoresCurso)
                        {
                            context.CursoInstructor.Remove(id);
                        }
                    }

                    // aqui actualiza los nuevos instrutores

                    CursoInstructor cursoInstructor;
                    foreach (var Id in request.ListaInstructor)
                    {
                        cursoInstructor = new CursoInstructor
                        {
                            CursoId = request.CursoId,
                            InstructorId = Id
                        };
                        this.context.CursoInstructor.Add(cursoInstructor);
                    }

                }

                // actualizar aqui el precio

                var precioEntidad = this.context.Precio.Where(x => x.CursoId == request.CursoId).FirstOrDefault();

                if (precioEntidad != null)
                {
                    precioEntidad.Promocion = request.Promocion;
                    precioEntidad.PrecioActual = request.Precio;
                }
                else
                {
                    precioEntidad = new Precio
                    {
                        CursoId = curso.CursoId,
                        PrecioActual = request.Precio,
                        Promocion = request.Promocion,
                        PrecioId = Guid.NewGuid()
                    };

                    this.context.Precio.Add(precioEntidad);
                }

                var respuesta = await this.context.SaveChangesAsync();

                if (respuesta == 0) throw new HandlerException(HttpStatusCode.InternalServerError, new
                {
                    msg = "No se pudo Actualizar el curso"
                }); //throw new Exception("Curso no encontrado");



                return Unit.Value; // este flag indica que todo fue exitoso

            }
        }
    }
}