using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    // para la parte de seguridad y login se va usar identity core 
    public class Usuario: IdentityUser
    {
        public string NombreCompleto {get;set;}
    }
}