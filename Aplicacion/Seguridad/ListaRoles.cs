using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class ListaRoles
    {
        public class ListaRolesData : IRequest<List<IdentityRole>>
        {

        }

        public class Handler : IRequestHandler<ListaRolesData, List<IdentityRole>>
        {

            private readonly CursosOnLineContext _context;

            public Handler(CursosOnLineContext context)
            {
                this._context = context;
            }

            public async Task<List<IdentityRole>> Handle(ListaRolesData request, CancellationToken cancellationToken)
            {
                var roles = await _context.Roles.ToListAsync();
                return roles;
            }
        }
    }
}