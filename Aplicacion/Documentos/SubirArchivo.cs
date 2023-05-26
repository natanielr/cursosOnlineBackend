using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.HandlerError;
using Aplicacion.Seguridad;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Documentos
{
    public class SubirArchivo
    {
        public class SubirArchivoData : IRequest
        {

            public string EntidadId { get; set; }
            public string Data { get; set; } // viene en base64 
            public string Nombre { get; set; }
            public string Extension { get; set; }
        }

        public class Handler : IRequestHandler<SubirArchivoData>
        {

            private readonly CursosOnLineContext context;
            public Handler(CursosOnLineContext _context)
            {
                this.context = _context;
            }
            public async Task<Unit> Handle(SubirArchivoData request, CancellationToken cancellationToken)
            {
                var documento = await context.Documento.Where(x => x.EntidadId == new Guid(request.EntidadId)).FirstOrDefaultAsync();

                if (documento == null)
                {
                    var imagen = new Documento
                    {
                        Contenido = Convert.FromBase64String(request.Data), // la tabla recibe la imagen en bytes
                        Nombre = request.Nombre,
                        Extension = request.Extension,
                        DocumentoId = Guid.NewGuid(),
                        FechaCreacion = DateTime.UtcNow,
                        EntidadId = new Guid(request.EntidadId)
                    };

                    context.Documento.Add(imagen);
                }
                else
                {
                    documento.Contenido = Convert.FromBase64String(request.Data);
                    documento.Nombre = request.Nombre;
                    documento.Extension = request.Extension;
                }

                var resultado = await context.SaveChangesAsync();

                if(resultado > 0) return Unit.Value;


                throw new HandlerException(System.Net.HttpStatusCode.InternalServerError,new {
                    msg ="No se pudo actualizar la imagen del perfil"
                });
            }
        }
    }
}