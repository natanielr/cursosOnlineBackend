using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using Dominio;
using MediatR;
using Persistencia;
using AutoMapper;
using Aplicacion.Dto;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoId : IRequest<CursoDto>
        {
            public Guid Id { get; set; } // esta clase va representar el cuerpo de entrada de la peticion http
        }
        public class Handler : IRequestHandler<CursoId, CursoDto>
        {
            private readonly CursosOnLineContext context;
            private readonly IMapper mapper;

            public Handler(CursosOnLineContext _context, IMapper _mapper)
            {
                this.context = _context;
                this.mapper = _mapper;
            }

            public async Task<CursoDto> Handle(CursoId request, CancellationToken cancellationToken)
            {
                var curso = await this.context.Curso
                    .Include(x => x.ComentarioLista)
                    .Include(x => x.PrecioCurso)
                    .Include(x => x.InstructoresLink)
                    .ThenInclude(z => z.Instructor)
                    .FirstOrDefaultAsync(cr => cr.CursoId == request.Id);

                if (curso == null) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "No se encontro el curso"
                });

                var cursoDto = this.mapper.Map<Curso, CursoDto>(curso);

                return cursoDto;
            }
        }
    }
}