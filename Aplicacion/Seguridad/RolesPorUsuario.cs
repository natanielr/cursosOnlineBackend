using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolesPorUsuario
    {
        public class RolesPorUsuarioData : IRequest<List<string>>
        {
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<RolesPorUsuarioData, List<string>>
        {
            private readonly UserManager<Usuario> _userManager;

            public Handler(UserManager<Usuario> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<string>> Handle(RolesPorUsuarioData request, CancellationToken cancellationToken)
            {
                var usuario = await _userManager.FindByIdAsync(request.UserId);

                if (usuario == null) throw new HandlerError.HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "No existe el usuario"
                });

                return (await _userManager.GetRolesAsync(usuario)).ToList();
            }
        }
    }
}