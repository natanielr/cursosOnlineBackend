using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Dto;
using AutoMapper;
using Dominio;

namespace Aplicacion
{

    // esta clase maneja el mapeop de datos de las clases que representan las entidades con las clases que van a mostrar los datos de salida
    // al cliente, o sea los Dto
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            CreateMap<Curso,CursoDto>()
            // mapear los intructores para cursosdto
            .ForMember(x => x.Instructores, y => y.MapFrom( z=> z.InstructoresLink
            .Select(a => a.Instructor).ToList()))
            .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista))
            .ForMember(x => x.Precio, y => y.MapFrom(z => z.PrecioCurso));

            /*
                Explicacion del mapeo.

                Como cursoDto necesita obtener la informacion de los instructores entonces debe mapear la informacion del instructor.
                EN el forMember la propiedad Instructores es el punte entre CursosDto y CursoInstructor, luego llama a la propiedad
                INstructor que hace la conexion entre cursosDto, cursosInstructor e Intructor para mapear la informacion de instructor
            */
            // aqui se van mapeando las clases con los dtos por lo que, en vez de devolver todos los datos de las entidades 
            // se devuelve solamente lo que esta asignado en las clases dto
            CreateMap<CursoInstructor,CursoInstructorDto>();
            CreateMap<Instructor,InstructorDto>();
            CreateMap<Comentario,ComentarioDto>();
            CreateMap<Precio,PrecioDto>();
        }
    }
}