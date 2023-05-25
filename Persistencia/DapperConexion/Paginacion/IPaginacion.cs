using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public interface IPaginacion
    {
        Task<PaginacionModel> devolverPaginacion(
            string storeProcedure, // nombre del sp que va llamar
            int numeroPagina,
            int cantidadElementos,
            IDictionary<string, object> parametrosFiltro, // en Javascript seria como enviar un objeto literal como parametro de busqueda
            string ordeneamientoColumna
        );
    }
}