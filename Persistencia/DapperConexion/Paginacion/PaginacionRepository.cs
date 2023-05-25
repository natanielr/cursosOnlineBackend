using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepository : IPaginacion
    {

        private IFactoryConnection _factoryConnection;
        public PaginacionRepository(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }
        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFiltro, string ordernamiento)
        {
            PaginacionModel paginacionModel = new PaginacionModel();
            List<IDictionary<string, object>> resultData = null; // devuelve los datos como JSON
            int totalRecords = 0, totalPaginas = 0;
            try
            {

                var connection = _factoryConnection.GetConnection();

                DynamicParameters parametrosSp = new DynamicParameters();

                // leer los filtros de busqueda de forma dinamica 

                foreach(var param in parametrosFiltro) {
                    parametrosSp.Add("@"+param.Key, param.Value);
                }

                parametrosSp.Add("@numeroPagina", numeroPagina);
                parametrosSp.Add("@cantidadElementos", cantidadElementos);
                parametrosSp.Add("@ordenamiento", ordernamiento);

                // declarar los parametros de salida de los sps 

                parametrosSp.Add("@totalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parametrosSp.Add("@totalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);

                //

                var result = await connection.QueryAsync(storeProcedure, parametrosSp, commandType: CommandType.StoredProcedure);
                resultData = result.Select(x => (IDictionary<string, object>)x).ToList(); // convertir cada uno de los elementos en un tipo generico JSOn
                
                // se cargan los registros 
                paginacionModel.listaRegistros = resultData;

                // retornar la informacion de los parametros de salida de los sps 
                paginacionModel.totalPaginas = parametrosSp.Get<int>("@totalPaginas");
                paginacionModel.totalRegistros = parametrosSp.Get<int>("@totalRecords");
            }
            catch (System.Exception e)
            {   
                Console.WriteLine("Hubo un error", e.Message);
                throw new Exception("Hubo un error", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }

            return paginacionModel;
        }
    }
}