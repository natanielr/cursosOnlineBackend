using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructores;

namespace Aplicacion.Instructores
{
    public class EliminarInsstructor
    {
        public class EliminarInstructorData : IRequest
        {
            public Guid Id { get; set; }
        }

        public class EliminarInstructorDataDataValidaciones : AbstractValidator<EliminarInstructorData>
        {
            public EliminarInstructorDataDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<EliminarInstructorData>
        {
            public readonly IInstructor _instructor;

            public Handler(IInstructor instructor)
            {
                _instructor = instructor;
            }

            public async Task<Unit> Handle(EliminarInstructorData request, CancellationToken cancellationToken)
            {
                var resultado = await _instructor.EliminarInstructor(request.Id);

                if (resultado == 0) return Unit.Value;

                throw new HandlerException(HttpStatusCode.InternalServerError, new
                {
                    msg = "No se pudo eliminar el Instructor"
                });
            }
        }
    }
}

