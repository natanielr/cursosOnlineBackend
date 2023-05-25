using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Dto
{
    public class CursoDto
    {
        public Guid CursoId {get;set;} // recomendado para claves primarias porque C# lo genera automaticamente
        public string Titulo {get;set;}
        public string Descripcion {get;set;}
        public DateTime FechaPublicacion {get;set;}
        public DateTime? FechaCreacion {get;set;}
        public byte[] FotoPortada {get;set;}
        public ICollection<InstructorDto> Instructores {get;set;}
        public PrecioDto Precio{get;set;}
        public ICollection<ComentarioDto> Comentarios{get;set;}
    }
}