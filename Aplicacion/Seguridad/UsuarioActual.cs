using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class UsuarioActualData : IRequest<UsuarioResponse> { }

        public class Handler : IRequestHandler<UsuarioActualData, UsuarioResponse>
        {

            private readonly UserManager<Usuario> userManager;
            private readonly IJwtGenerador token;
            private readonly IUsuarioSesion usuarioSesion;
            private readonly CursosOnLineContext context;

            public Handler(UserManager<Usuario> _userManager, IJwtGenerador _token, IUsuarioSesion _usuarioSesion, CursosOnLineContext _context)
            {
                this.userManager = _userManager;
                this.token = _token;
                this.usuarioSesion = _usuarioSesion;
                this.context = _context;
            }
            public async Task<UsuarioResponse> Handle(UsuarioActualData request, CancellationToken cancellationToken)
            {
                var usuario = await userManager.FindByNameAsync(usuarioSesion.ObtenerUsuarioActual()); // obtiene el usuario de la sesion actual
                var roles = (await userManager.GetRolesAsync(usuario)).ToList(); // obtener los roles para agregarlos al token
                var existeImagen = await context.Documento.Where(x => x.EntidadId == new Guid(usuario.Id)).FirstOrDefaultAsync();
                // y los busca en la bd

                var response = new UsuarioResponse
                {
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    token = token.crearToken(usuario, roles),
                    Imagen = null
                };

                if(existeImagen != null) {
                    response.ImagenPerfil = new ImagneGeneral {
                        Data = Convert.ToBase64String(existeImagen.Contenido),
                        Extension = existeImagen.Extension,
                        Nombre = existeImagen.Nombre
                    };
                }

                return response;
            }
        }
    }
}