using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using Aplicacion.Dto;
using AutoMapper;

namespace Aplicacion.Cursos
{
    public class Consulta
    {
        public class ListaCursos : IRequest<List<CursoDto>> { } // El tipo de informacion que va devolver la llamada http
        public class Handler : IRequestHandler<ListaCursos, List<CursoDto>>
        { // este es el manejador que devuelve el tipo de informacion y el formato

            private readonly CursosOnLineContext context;
            private readonly IMapper mapper;
            public Handler(CursosOnLineContext _context, IMapper _mapper)
            {
                this.context = _context;
                this.mapper = _mapper;
            }
            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                var cursos = await this.context.Curso
                    .Include(x => x.ComentarioLista)
                    .Include(x => x.PrecioCurso)
                    .Include(x => x.InstructoresLink)
                    .ThenInclude(x=>x.Instructor) // el thenInclude se agrega cuando la relacion de las entidades es de muchos a muchos
                    .ToListAsync();

                // se usa el mapper para obtener y devolver la informacion con los dtos
                // Map<List<Curso>,List<CursoDto>> donde List<Curso> es el tipo de dato de entrada y List<CursoDto> es la salida de los datos
                // y el valor de la variable cursos es la informacion en crudo

                var cursosDto = this.mapper.Map<List<Curso>,List<CursoDto>>(cursos);
                    
                return cursosDto;
            }
        }
    }
}