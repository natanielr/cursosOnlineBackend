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

namespace Aplicacion.Comentarios
{
    public class NuevoComentario
    {
        public class NuevoComentarioData : IRequest
        {
            public string Alumno { get; set; }
            public int? Puntaje { get; set; }
            public string Descripcion { get; set; }
            public Guid CursoId { get; set; }
        }

        public class NuevoComentarioDataValidaciones : AbstractValidator<NuevoComentarioData> {
            public NuevoComentarioDataValidaciones() {
                RuleFor(x => x.Alumno).NotEmpty();
                //RuleFor(x => x.Puntaje).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.CursoId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<NuevoComentarioData>
        {
            private readonly CursosOnLineContext context;

            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }

            public async Task<Unit> Handle(NuevoComentarioData request, CancellationToken cancellationToken)
            {
                var comentario = new Comentario {
                    ComentarioId = Guid.NewGuid(),
                    Alumno = request.Alumno,
                    Puntaje = request.Puntaje ?? null,
                    Descripcion = request.Descripcion,
                    CursoId = request.CursoId,
                    FechaCreacion = DateTime.UtcNow
                };

                context.Comentario.Add(comentario);
                var resultado = await  context.SaveChangesAsync();

                if(resultado > 0) return Unit.Value;
                throw new HandlerException(HttpStatusCode.InternalServerError,new {
                    msg = "No se pudo registrar el comentario"
                });
            }
        }
    }
}