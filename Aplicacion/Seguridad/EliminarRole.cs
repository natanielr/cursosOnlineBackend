using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Aplicacion.Seguridad
{
    public class EliminarRole
    {
        public class EliminarRoleData : IRequest
        {
            public string Id {get;set;}
        }

        public class EliminarRoleDataValidaciones : AbstractValidator<EliminarRoleData>
        {
            public EliminarRoleDataValidaciones()
            {
                 RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<EliminarRoleData>
        {

            private readonly RoleManager<IdentityRole> _role;

            public Handler( RoleManager<IdentityRole> role) {
                this._role = role;
            }
            
            public async Task<Unit> Handle(EliminarRoleData request, CancellationToken cancellationToken)
            {
                var role = await _role.FindByIdAsync(request.Id);

                if (role == null) throw new HandlerException(HttpStatusCode.BadRequest,  new {
                    msg = "No existe el role"
                });

                var resultado = await _role.DeleteAsync(role);

                if(resultado.Succeeded) return Unit.Value;
                throw new HandlerException(HttpStatusCode.BadRequest,  new {
                    msg = "No se pudo eliminar el role"
                });
            }
        }
    }
}