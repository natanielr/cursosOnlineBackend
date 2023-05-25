using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioRoleEliminar
    {
        public class UsuarioRoleEliminarData : IRequest
        {
            public string UserId { get; set; }
            public string RoleId { get; set; }
        }

        public class UsuarioRoleDataValidaciones : AbstractValidator<UsuarioRoleEliminarData>{
            public UsuarioRoleDataValidaciones(){
                RuleFor(x => x.RoleId).NotEmpty();
                RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<UsuarioRoleEliminarData>
        {

            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<Usuario> _userManager;


            public Handler(RoleManager<IdentityRole> roleManager,UserManager<Usuario> userManager)
            {
                this._roleManager = roleManager;
                this._userManager = userManager;
            }
            
            public async Task<Unit> Handle(UsuarioRoleEliminarData request, CancellationToken cancellationToken)
            {


                Console.WriteLine("userId",request.UserId);
                Console.WriteLine("roleId",request.RoleId);
                

                var role = await _roleManager.FindByIdAsync(request.RoleId);
                var usuario =  await _userManager.FindByIdAsync(request.UserId);

                if (role == null || usuario == null) throw new HandlerError.HandlerException(HttpStatusCode.NotFound,new {
                    msg = "No se encontraron resultados"
                });

                var resultado = await _userManager.RemoveFromRoleAsync(usuario,role.Name);

                if(resultado.Succeeded) return Unit.Value;

                throw new HandlerError.HandlerException(HttpStatusCode.InternalServerError,new {
                    msg = "No se pudo eliminar el role al usuario"
                });

            }
        }
    }
}