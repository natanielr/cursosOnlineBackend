using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class NuevoRole
    {
        public class NuevoRoleData: IRequest {
            public string Nombre {get;set;}
        }

        public class NuevoRoleDataValidaciones : AbstractValidator<NuevoRoleData> {
            public NuevoRoleDataValidaciones() {
                RuleFor(x => x.Nombre).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<NuevoRoleData>
        {

            private readonly RoleManager<IdentityRole> _role;

            public Handler( RoleManager<IdentityRole> role) {
                this._role = role;
            }
            public async Task<Unit> Handle(NuevoRoleData request, CancellationToken cancellationToken)
            {
                var role = await _role.FindByNameAsync(request.Nombre);
                if (role != null) throw new HandlerError.HandlerException(HttpStatusCode.BadRequest,new {msg ="Ya existe el role"});

                var resultado = await _role.CreateAsync(new IdentityRole(request.Nombre)); // aqui crea el nuevo role

                if (resultado.Succeeded) return Unit.Value;
                throw new HandlerError.HandlerException(HttpStatusCode.BadRequest,new {msg ="No se pudo agregar el role"});
            }
        }
    }
}