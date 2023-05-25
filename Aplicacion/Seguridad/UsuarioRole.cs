using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using System.Net;
using MediatR;
using Persistencia;
using Microsoft.AspNetCore.Identity;
using Dominio;

namespace Aplicacion.Seguridad
{
    public class UsuarioRole
    {
        public class UsuarioRoleData: IRequest {// cuando es una operacion de busqueda se debe indicar en la interfaz generica el tipo
        // de valor que va devolver esta solicitud
            public string UserId {get;set;}
            public string RoleId {get;set;}
        }

        public class UsuarioRoleDataValidaciones: AbstractValidator<UsuarioRoleData>{
            public UsuarioRoleDataValidaciones() {
                RuleFor(x => x.RoleId).NotEmpty();
                RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<UsuarioRoleData>
        {

       
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<Usuario> _userManager;

            public Handler(CursosOnLineContext context,RoleManager<IdentityRole> roleManager,UserManager<Usuario> userManager)
            {
                this._roleManager = roleManager;
                this._userManager = userManager;
            }
            
            public async Task<Unit> Handle(UsuarioRoleData request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                var usuario =  await _userManager.FindByIdAsync(request.UserId);

                if (role == null || usuario == null) throw new HandlerError.HandlerException(HttpStatusCode.NotFound,new {
                    msg = "No se encontraron resultados"
                });

                var resultado = await _userManager.AddToRoleAsync(usuario,role.Name);

                if(resultado.Succeeded) return Unit.Value;

                throw new HandlerError.HandlerException(HttpStatusCode.InternalServerError,new {
                    msg = "No se pudo agregar el role al usuario"
                });
            }
        }
    }
}