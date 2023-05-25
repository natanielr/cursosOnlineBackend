using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Dto
{
    public class ComentarioDto
    {
        public Guid ComentarioId {get;set;} // recomendado para claves primarias porque C# lo genera automaticamente
        public string Alumno{get;set;}
        public int? Puntaje{get;set;}
        public string Descripcion {get;set;}
        public DateTime? FechaCreacion {get;set;}
        public Guid CursoId{get;set;}
    }
}