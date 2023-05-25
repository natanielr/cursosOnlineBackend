using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Aplicacion.Documentos
{
    public class ObtenerArchivo
    {
        public class ObtenerArchivoData: IRequest<ArchivoGenerico> {
            public Guid? UsuarioId {get;set;}
        }

        public class Handler : IRequestHandler<ObtenerArchivoData, ArchivoGenerico>
        {
            private readonly CursosOnLineContext context;
            public  Handler(CursosOnLineContext _context)
            {
                context = _context;
            }

            public async Task<ArchivoGenerico> Handle(ObtenerArchivoData request, CancellationToken cancellationToken)
            {
                var documento = await context.Documento.Where(x => x.UsuarioId == request.UsuarioId).FirstOrDefaultAsync();

                if(documento != null) {

                    var response = new ArchivoGenerico {
                        DocumentoId = documento.DocumentoId,
                        UsuarioId = documento.UsuarioId,
                        Nombre = documento.Nombre,
                        Extension = documento.Extension,
                        Data = Convert.ToBase64String(documento.Contenido) 
                    };

                    return response;

                } else {
                    throw new HandlerError.HandlerException(HttpStatusCode.NotFound, new {
                        msg="No hay resultados"
                    });
                }
            }
        }
    }
}