using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using FluentValidation;
using MediatR;
using Persistencia;
using System.Net;

namespace Aplicacion.Comentarios
{
    public class EliminarComentario
    {
        public class EliminarComentarioData : IRequest
        {
            public Guid Id { get; set; }
        }

        public class EliminarComentarioDataValidaciones : AbstractValidator<EliminarComentarioData>
        {

            public EliminarComentarioDataValidaciones()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<EliminarComentarioData>
        {
            private readonly CursosOnLineContext context;

            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }

            public async Task<Unit> Handle(EliminarComentarioData request, CancellationToken cancellationToken)
            {
                var comentario = context.Comentario.Where(x => x.ComentarioId == request.Id).FirstOrDefault();

                if (comentario == null) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "El comentario no existe"
                });

                context.Remove(comentario);

                var respuesta = await context.SaveChangesAsync();

                if (respuesta == 0)
                {
                    throw new HandlerException(HttpStatusCode.InternalServerError, new
                    {
                        msg = "No se pudo eliminar el comentario"
                    });
                }

                return Unit.Value;
            }
        }
    }
}