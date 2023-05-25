using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructores
{
    public interface IInstructor
    {
        Task<IEnumerable<InstructorModel>> ObtenerLista();
        Task<InstructorModel> ObtenerPorId(Guid id);
        Task<int> NuevoInstructor(InstructorModel instructor);
        Task<string> ActualizarInstructor(InstructorModel instructor);
        Task<int> EliminarInstructor(Guid id);
    }
}