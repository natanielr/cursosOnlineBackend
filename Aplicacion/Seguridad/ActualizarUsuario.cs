using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistencia;
using Aplicacion.HandlerError;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Seguridad
{
    public class ActualizarUsuario
    {
        public class ActualizarUsuarioData : IRequest<UsuarioResponse>
        {
            public string Nombre { get; set; }
            public string Apellidos { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public ImagneGeneral ImagenPerfil { get; set; } = null;
        }

        public class ActualizarUsuarioDataValidaciones : AbstractValidator<ActualizarUsuarioData>
        {

            public static readonly string EmailValidation_Regex = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            public ActualizarUsuarioDataValidaciones()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Email).EmailAddress().Matches(EmailValidation_Regex);
            }
        }

        public class Handler : IRequestHandler<ActualizarUsuarioData, UsuarioResponse>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly CursosOnLineContext _context;
            private readonly IJwtGenerador _tokenManager;
            private readonly IPasswordHasher<Usuario> _passwordHasher;
            public Handler(UserManager<Usuario> userManager, CursosOnLineContext context, IJwtGenerador tokenManager, IPasswordHasher<Usuario> passwordHasher)
            {
                this._userManager = userManager;
                this._context = context;
                this._tokenManager = tokenManager;
                this._passwordHasher = passwordHasher;
            }

            public async Task<UsuarioResponse> Handle(ActualizarUsuarioData request, CancellationToken cancellationToken)
            {

                Documento imagen;
                var usuario = await _userManager.FindByNameAsync(request.UserName);
                if (usuario == null) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "No existe el usuario"
                });

                // validar que el Email no exista en otro usuario
                var email = await _context.Users.Where(x => x.Email == request.Email && x.UserName != request.UserName).AnyAsync();
                if (email) throw new HandlerException(HttpStatusCode.NotFound, new
                {
                    msg = "Ya hay un perfil con los datos ingresados"
                });

                if (request.ImagenPerfil != null)
                {
                    var existeImagen = await _context.Documento.Where(x => x.EntidadId == new Guid(usuario.Id)).FirstOrDefaultAsync();

                    if (existeImagen == null)
                    { // ya tiene una imagen
                        imagen = new Documento
                        {
                            Contenido = Convert.FromBase64String(request.ImagenPerfil.Data), // la tabla recibe la imagen en bytes
                            Nombre = request.ImagenPerfil.Nombre,
                            Extension = request.ImagenPerfil.Extension,
                            EntidadId = new Guid(usuario.Id),
                            DocumentoId = Guid.NewGuid(),
                            FechaCreacion = DateTime.UtcNow
                        };

                        _context.Documento.Add(imagen);
                    }
                    else
                    {
                        existeImagen.Contenido = Convert.FromBase64String(request.ImagenPerfil.Data);
                        existeImagen.Nombre = request.ImagenPerfil.Nombre;
                        existeImagen.Extension = request.ImagenPerfil.Extension;

                    }
                }

                // actualizar los datos del usuario
                usuario.NombreCompleto = String.Concat(request.Nombre, ' ', request.Apellidos);
                usuario.UserName = request.UserName;
                usuario.Email = request.Email;
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);

                // actualizar la informacion del usuario 
                var resultado = await _userManager.UpdateAsync(usuario);
                if (resultado.Succeeded)
                {
                    var roles = (await _userManager.GetRolesAsync(usuario)).ToList();
                    var imagenActual = await _context.Documento.Where(x => x.EntidadId == new Guid(usuario.Id)).FirstOrDefaultAsync();

                    var response = new UsuarioResponse
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Email = usuario.Email,
                        UserName = usuario.UserName,
                        token = _tokenManager.crearToken(usuario, roles),
                    };

                    if (imagenActual != null)
                    {
                        response.ImagenPerfil = new ImagneGeneral
                        {
                            Data = Convert.ToBase64String(imagenActual.Contenido),
                            Extension = imagenActual.Extension,
                            Nombre = imagenActual.Nombre
                        };

                    }
                    // generar token y roles

                    return response;
                }

                throw new HandlerException(HttpStatusCode.InternalServerError, new
                {
                    msg = "No se pudo actualizar la informacion"
                });
            }
        }
    }
}