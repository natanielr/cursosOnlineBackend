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
    public class NuevoInstructor
    {
        public class NuevoInstructorData : IRequest
        {
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Grado { get; set; }
        }

        public class NuevoInstructorDataValidaciones : AbstractValidator<NuevoInstructorData>
        {
            public NuevoInstructorDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Grado).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<NuevoInstructorData>
        {

            public readonly IInstructor _instructor;

            public Handler(IInstructor instructor)
            {
                _instructor = instructor;
            }

            public async Task<Unit> Handle(NuevoInstructorData request, CancellationToken cancellationToken)
            {
                

                var estado = await _instructor.NuevoInstructor(new InstructorModel
                {
                    InstructorId = Guid.NewGuid(),
                    Nombre = request.Nombre,
                    Apellidos = request.Apellidos,
                    Grado = request.Grado
                });


                if (estado > 0) return Unit.Value;

                throw new HandlerException(HttpStatusCode.InternalServerError, new
                {
                    msg = "No se pudo registrar el Instructor 2"
                });
            }
        }
    }

}