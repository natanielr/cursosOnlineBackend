using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.HandlerError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class Login
    {
        public class LoginData : IRequest<UsuarioResponse> //Aqui se puede cambiar el tipo de clase generica para 
        //devolver un tipo de respuesta personalizada 
        // clase que contiene los datos de entrada del request http
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginDataValidaciones : AbstractValidator<LoginData> // clase de validacion de datos
        {

            public static readonly string EmailValidation_Regex = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

            public LoginDataValidaciones()
            {
                // se agrega una clase para manejar las validaciones con FLuentValidation
                RuleFor(x => x.Email).EmailAddress().Matches(EmailValidation_Regex);
                RuleFor(x => x.Password).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<LoginData, UsuarioResponse> //Aqui se puede cambiar el tipo de clase generica para 
        //devolver un tipo de respuesta personalizada
        {

            private readonly UserManager<Usuario> _userManager;
            private readonly SignInManager<Usuario> _signInManager;
            private readonly IJwtGenerador _tokenGenerador;
            private readonly CursosOnLineContext context;
            public Handler(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtGenerador tokenGenerador, CursosOnLineContext _context)

            {
                this._signInManager = signInManager;
                this._userManager = userManager;
                this._tokenGenerador = tokenGenerador;
                this.context = _context;
            }

            public async Task<UsuarioResponse> Handle(LoginData request, CancellationToken cancellationToken)
            // dentro de la clase generica del userManager se va devolver la respuesta ya sea del modelo o de
            {
                var usuario = await this._userManager.FindByEmailAsync(request.Email);
                if (usuario == null)
                {
                    throw new HandlerException(HttpStatusCode.Unauthorized, new
                    {
                        msg = "Usuario y/o Contraseña incorrectos"
                    });
                }

                //comparar la contraseña 
                var resultado = await this._signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);


                if (resultado.Succeeded)
                { // entra aqui si el login fue correcto genera el token agregando los roles 



                    var roles = (await _userManager.GetRolesAsync(usuario)).ToList();

                    // traer la imagen

                    var existeImagen = await context.Documento.Where(x => x.EntidadId == new Guid(usuario.Id)).FirstOrDefaultAsync();

                    var response = new UsuarioResponse
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Email = usuario.Email,
                        token = this._tokenGenerador.crearToken(usuario, roles),
                        Imagen = "",
                        UserName = usuario.UserName
                    };

                    if (existeImagen != null)
                    {
                        response.ImagenPerfil = new ImagneGeneral
                        {
                            Data = Convert.ToBase64String(existeImagen.Contenido),
                            Extension = existeImagen.Extension,
                            Nombre = existeImagen.Nombre
                        };
                    }

                    return response;
                }

                throw new HandlerException(HttpStatusCode.Unauthorized, new
                {
                    msg = "Usuario y/o Contraseña incorrectos"
                });
            }
        }
    }
}