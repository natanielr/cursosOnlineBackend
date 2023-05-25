using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructores;

namespace Aplicacion.Instructores
{
    public class ConsultaId
    {
        public class InstructorId : IRequest<InstructorModel>
        {
            public Guid Id {get;set;}
        }

        public class NuevoCursoDataValidaciones : AbstractValidator<InstructorId>
        {
            public NuevoCursoDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Id).NotEmpty();   
            }
        }

        public class Handler : IRequestHandler<InstructorId, InstructorModel>
        {   

            private readonly IInstructor _instructor;

            public Handler(IInstructor instructor)
            {
                _instructor = instructor;
            }
            public async Task<InstructorModel> Handle(InstructorId request, CancellationToken cancellationToken)
            {
                return await _instructor.ObtenerPorId(request.Id);
            }
        }
    }
}