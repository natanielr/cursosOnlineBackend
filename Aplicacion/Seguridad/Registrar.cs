using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.HandlerError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class RegistrarData : IRequest<UsuarioResponse>
        {

            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string UserName {get;set;}
        }

        public class RegistrarDataValidaciones : AbstractValidator<RegistrarData> // clase de validacion de datos
        {

            public static readonly string EmailValidation_Regex = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            public RegistrarDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Email).EmailAddress().Matches(EmailValidation_Regex);
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<RegistrarData, UsuarioResponse> //Aqui se puede cambiar el tipo de clase generica para 
        //devolver un tipo de respuesta personalizada
        {

            private readonly CursosOnLineContext context;
            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador token;

            public Handler(CursosOnLineContext _context, UserManager<Usuario> _userManager, IJwtGenerador _token)
            {
                this.context = _context;
                this.userManager = _userManager;
                this.token = _token;
            }
            public async Task<UsuarioResponse> Handle(RegistrarData request, CancellationToken cancellationToken)
            {

                var _existe = await this.context.Users.Where(x => x.Email == request.Email || x.UserName == request.UserName).AnyAsync();
                // con la funcion .AnyAsync() devuelve una respuesta boolenada asincrona indicando si exsite o no un registo con ese correo
                if (_existe)
                {
                    throw new HandlerException(HttpStatusCode.BadRequest, new
                    {
                        msg = "Ya hay una cuenta relacionada la información del email o nombre de usuario"
                    });
                }
                var usuario = new Usuario
                {
                    Email = request.Email,
                    NombreCompleto = request.Nombre + " " + request.Apellidos,
                    UserName = request.UserName
                };

                var nuevoUsuario = await this.userManager.CreateAsync(usuario, request.Password);
                

                if (nuevoUsuario.Succeeded)
                {
                    

                    return new UsuarioResponse
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Email = usuario.Email,
                        token = this.token.crearToken(usuario,null),
                        Imagen = "",
                        UserName = usuario.UserName
                    };
                }

                throw new HandlerException(HttpStatusCode.BadRequest, new
                {
                    msg = "No se pudo registrar el usuario"
                });
            }
        }
    }
}