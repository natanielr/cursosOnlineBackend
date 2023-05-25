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
    public class EditarInstructor
    {
         public class EditarInstructorData : IRequest
        {
            public Guid InstructorId { get; set; }
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Grado { get; set; }
        }

        public class EditarInstructorDataValidaciones : AbstractValidator<EditarInstructorData>
        {
            public EditarInstructorDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Grado).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<EditarInstructorData>
        {

            public readonly IInstructor _instructor;

            public Handler(IInstructor instructor)
            {
                _instructor = instructor;
            }
            public async Task<Unit> Handle(EditarInstructorData request, CancellationToken cancellationToken)
            {
                 var estado = await _instructor.ActualizarInstructor(new InstructorModel
                {
                    InstructorId = request.InstructorId,
                    Nombre = request.Nombre,
                    Apellidos = request.Apellidos,
                    Grado = request.Grado,
                });

                if (estado == "Se ha actualizado el instructor") return Unit.Value;

                throw new HandlerException(HttpStatusCode.InternalServerError, new
                {
                    msg = "No se pudo Actualizar el Instructor"
                });
            }
        }
    }
}