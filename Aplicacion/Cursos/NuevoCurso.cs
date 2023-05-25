using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
//using System.ComponentModel.DataAnnotations; // para poder agregar anotaciones que sirvan como validacion de campos
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class NuevoCurso
    {
        public class NuevoCursoData : IRequest
        {   // ErrorMessage para agregar un mensaje de error personalizado
           //[Required(ErrorMessage ="El título es requerido")]
            public string Titulo { get; set; }
            //[Required(ErrorMessage ="La descripción es requerida")]
            public string Descripcion { get; set; }
            //[Required(ErrorMessage = "La fecha de publicación es requerida")]
            public DateTime FechaPublicacion { get; set; }

            public List<Guid> ListaInstructor {get;set;}
            public decimal Precio {get;set;}
            public decimal Promocion {get;set;}
        }
        public class NuevoCursoDataValidaciones: AbstractValidator<NuevoCursoData> {
            public NuevoCursoDataValidaciones () {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
                RuleFor(x => x.Precio).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<NuevoCursoData>
        {
            private readonly CursosOnLineContext context;

            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }
            public async Task<Unit> Handle(NuevoCursoData request, CancellationToken cancellationToken)
            {
                // CancellationToken cancellationToken es un parametro que cancela una consulta a nivel de servidor cuando un servicio
                // dura mucho en responder y el cliente cancela el request desde el navegador
                var curso = new Curso{
                    CursoId = Guid.NewGuid(),
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow
                };

                this.context.Add(curso);
                //preguntar si se agrega algun instructor al curso
                Console.WriteLine("request.ListaInstructor",request.ListaInstructor);
                if(request.ListaInstructor != null) {
                   CursoInstructor cursoInstructor;
                    foreach(var Id in request.ListaInstructor ) {
                        cursoInstructor = new CursoInstructor {
                            CursoId = curso.CursoId,
                            InstructorId  = Id
                        };
                        this.context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                // aqui se debe guardar el precio
                var precioEntidad = new Precio {
                    CursoId = curso.CursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid()
                };

                this.context.Precio.Add(precioEntidad);

                var estado = await context.SaveChangesAsync();

                if(estado > 0) return Unit.Value;    
                throw new HandlerException(HttpStatusCode.InternalServerError,new {
                    msg = "No se pudo registrar el curso"
                });
            }
        }
    }
}