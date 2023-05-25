using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;

namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
        string crearToken(Usuario usuario,List<string> roles); // devuelve el token
    }
}