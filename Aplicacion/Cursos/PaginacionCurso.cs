using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Paginacion;

namespace Aplicacion.Cursos
{
    public class PaginacionCurso
    {
        public class PaginacionCursoData : IRequest<PaginacionModel> {
            public string Titulo {get;set;}
            public int NumeroPagina {get;set;} = 1;
            public int CantidadElementos {get;set;} = 10;
        }

        public class Handler : IRequestHandler<PaginacionCursoData, PaginacionModel>
        {

            private IPaginacion _paginacionRepository;

            public Handler(IPaginacion paginacionRepository) {
                this._paginacionRepository = paginacionRepository;
            }
            public async Task<PaginacionModel> Handle(PaginacionCursoData request, CancellationToken cancellationToken)
            {

                var procedure ="sp_Obtener_Cursos_Paginados";
                var ordenamiento = "Titulo";

                // generar objetos genericos 

                var parametros = new Dictionary<string, object>();
                parametros.Add("NombreCurso",request.Titulo);

                return await _paginacionRepository.devolverPaginacion(procedure,request.NumeroPagina,request.CantidadElementos,parametros,ordenamiento);
            }
        }
    }
}