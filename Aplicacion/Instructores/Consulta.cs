using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Dto;
using MediatR;
using Persistencia.DapperConexion.Instructores;

namespace Aplicacion.Instructores
{
    public class Consulta
    {
        public class ListaInstructores : IRequest<List<InstructorModel>>
        {
        }

        public class Handler : IRequestHandler<ListaInstructores, List<InstructorModel>>
        {
            private readonly IInstructor _instructor;

            public Handler(IInstructor instructor)
            {
                _instructor = instructor;
            }

            public async Task<List<InstructorModel>> Handle(ListaInstructores request, CancellationToken cancellationToken)
            {
                var instructores = await _instructor.ObtenerLista();
                return instructores.ToList(); // .ToList para convertir de IEnumerable a List
            }
        }
    }
}