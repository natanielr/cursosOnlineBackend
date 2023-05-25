using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Seguridad
{
    public class UsuarioResponse
    {
        public string NombreCompleto {get;set;}
        public string token {get;set;}
        public string Email {get;set;}
        public string UserName {get;set;}
        public string Imagen {get;set;}
        public ImagneGeneral ImagenPerfil {get;set;}
    }
}