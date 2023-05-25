using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Aplicacion.HandlerError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class EliminarCurso
    {
        public class EliminarCursoData : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<EliminarCursoData>
        {
            private readonly CursosOnLineContext context;

            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }

            public async Task<Unit> Handle(EliminarCursoData request, CancellationToken cancellationToken)
            {
                var curso = await context.Curso.FindAsync(request.Id);

                if (curso == null) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "No se encontro el curso"
                }); //throw new Exception("Curso no encontrado");

                var instructoresCurso = context.CursoInstructor.Where(x => x.CursoId == request.Id).ToList();

                // agregar los nuevos instructores

                if (instructoresCurso?.Count > 0)
                {
                    // aqui se borran los instructores

                    foreach (var id in instructoresCurso)
                    {
                        context.CursoInstructor.Remove(id);
                    }
                }

                // eliminar el precio

                var precioEntidad = this.context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();

                if (precioEntidad != null)
                {
                    context.Precio.Remove(precioEntidad);
                }

                //eliminar los comentarios
                var comentariosDB = context.Comentario.Where(x => x.CursoId == request.Id).ToList();

                if (comentariosDB?.Count > 0)
                {
                    foreach (var comentario in comentariosDB)
                    {
                        context.Comentario.Remove(comentario);
                    }
                }

                context.Remove(curso);

                var respuesta = await context.SaveChangesAsync();

                if (respuesta == 0)
                {
                    throw new HandlerException(HttpStatusCode.InternalServerError, new
                    {
                        msg = "No se pudo eliminar el curso"
                    }); //throw new Exception("Curso no encontrado");
                }

                return Unit.Value;
            }
        }
    }
}