using System;

namespace Dominio
{
    public class Comentario
    {
        public Guid ComentarioId {get;set;} // recomendado para claves primarias porque C# lo genera automaticamente
        public string Alumno{get;set;}
        public int? Puntaje{get;set;}
        public string Descripcion {get;set;}
        public DateTime? FechaCreacion {get;set;}
        public Guid CursoId{get;set;}
        public Curso Curso {get;set;}

    }
}