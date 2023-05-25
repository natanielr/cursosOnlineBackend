using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
namespace Persistencia.DapperConexion.Instructores
{
    public class InstructorRepository : IInstructor
    {
        private IFactoryConnection _factoryConnection;
        public InstructorRepository(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }
        public async Task<string> ActualizarInstructor(InstructorModel instructor)
        {

            try
            {
                var procedure = "sp_obtener_Actualizar_Instructor";
                var connection = _factoryConnection.GetConnection();

                _factoryConnection.CloseConnection();
                var resultado = (await connection.QueryAsync<string>(procedure, new
                {
                    InstructorId = instructor.InstructorId,
                    Nombre = instructor.Nombre,
                    Apellidos = instructor.Apellidos,
                    Grado = instructor.Grado
                }, commandType: CommandType.StoredProcedure)).ToList();

                _factoryConnection.CloseConnection();
                return resultado.FirstOrDefault();
            }
            catch (System.Exception e)
            {
                throw new Exception("No se pudo registrar el nuevo instructor", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }
        }

        public async Task<int> EliminarInstructor(Guid id)
        {
            try
            {
                var procedure = "sp_obtener_Eliminar_Instructor";
                var connection = _factoryConnection.GetConnection();

                _factoryConnection.CloseConnection();
                var resultado = (await connection.QueryAsync<int>(procedure, new { InstructorId = id }, commandType: CommandType.StoredProcedure)).ToList();

                _factoryConnection.CloseConnection();
                return resultado.FirstOrDefault();
            }

            catch (System.Exception e)
            {

                throw new Exception("No se pudo Eliminar el nuevo instructor", e); ;
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }
        }

        public async Task<int> NuevoInstructor(InstructorModel instructor)
        {

            try
            {
                var procedure = "sp_Agregar_Instructor";
                var connection = _factoryConnection.GetConnection();

                _factoryConnection.CloseConnection();

                var resultado = await connection.ExecuteAsync(procedure, new
                {
                    InstructorId = instructor.InstructorId,
                    Nombre = instructor.Nombre,
                    Apellidos = instructor.Apellidos,
                    Grado = instructor.Grado
                }, commandType: CommandType.StoredProcedure);

                return resultado;
            }
            catch (System.Exception e)
            {

                throw new Exception("Error al registrar el instructor", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList;
            var procedure = "sp_Obtener_Instructores";

            try
            {
                var connection = _factoryConnection.GetConnection();
                instructorList = await connection.QueryAsync<InstructorModel>(procedure, null, commandType: CommandType.StoredProcedure);
                /*
                    connection.QueryAsync<InstructorModel>(procedure,null,commandType:CommandType.StoredProcedure);

                    obtiene la conexion, el QueryAsync es para ejecutar el query, el segundo parametro son los datos de entrada para ese query
                    y el tercer parametro es el tipo de query, que peude ser una consulta directa, un procedimiento almacenado, funcion.
                    La interfaz en el queryAsync indica el tipo de valor que va devolver

                */
            }
            catch (System.Exception e)
            {
                throw new Exception("Error en la transaccion", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }

            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            IEnumerable<InstructorModel> instructor;
            var procedure = "sp_obtener_Instructor_Por_id";

            try
            {
                var connection = _factoryConnection.GetConnection();
                instructor = await connection.QueryAsync<InstructorModel>(procedure, new { InstructorId = id }, commandType: CommandType.StoredProcedure);

            }
            catch (System.Exception e)
            {
                throw new Exception("Error en la transaccion", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }

            return instructor.ToList()[0];
        }
    }
}