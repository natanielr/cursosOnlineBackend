using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {

        private IDbConnection connection;
        private readonly IOptions<ConexionConfig> _config;

        public FactoryConnection(IOptions<ConexionConfig> config){
            _config = config;
        }
        public void CloseConnection()
        {
            if(connection?.State == ConnectionState.Open){
                connection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            //devolver una sola instancia de conexion 

            if(connection == null){
                connection = new SqlConnection(_config.Value.DefaultConnection);
            } 

            // comparar el estado de la conexion

            if(connection.State != ConnectionState.Open){
                connection.Open(); 
            }

            return connection;
        }
    }
}