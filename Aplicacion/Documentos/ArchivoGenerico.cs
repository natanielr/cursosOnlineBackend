using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Documentos
{
    public class ArchivoGenerico
    {
        public Guid? EntidadId {get;set;}
        public Guid? DocumentoId {get;set;}
        public string Nombre {get;set;}
        public string Data {get;set;}
        public string Extension {get;set;}
    }
}