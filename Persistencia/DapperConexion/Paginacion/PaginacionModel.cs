using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel 
    {
        //List<IDictionary<string,object>>  significa una lista de datos genericos de tipo JSON
        public List<IDictionary<string,object>> listaRegistros {get;set;}
        public int totalRegistros {get;set;}
        public int totalPaginas {get;set;}
    }
}