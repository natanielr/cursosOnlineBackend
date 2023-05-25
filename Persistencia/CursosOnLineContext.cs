using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistencia
{
    public class CursosOnLineContext : IdentityDbContext<Usuario> // siempre debe heredar de esta clase 
    {
        /*
            Normalmente la clase que guarda el context que es la conexion o la instancia con la bd, heredia de DbContext pero como en este 
            caso se va usar IdentityCore para manejar la seguridad de la aplicacion, entonces debe heredar de IdentityDbContext para implementar
            la entidad Usuario y manejar el login, las sessiones, roles y demas.
        */
        //crear un constructor 

        public CursosOnLineContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //actyivar la migracion de las entidades a la bd 

            base.OnModelCreating(modelBuilder);
            // indicar las llaves primarias a una entidad que tiene mas de una 

            modelBuilder.Entity<CursoInstructor>().HasKey(ci => new { ci.CursoId, ci.InstructorId });
        }

        // Convetir en entidades las clases que representan las entidades de la base de datos

        // estas son las instancias que el context llama para hacer las consultas con ef core
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Precio> Precio { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
        public DbSet<CursoInstructor> CursoInstructor { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Documento> Documento { get; set; }

    }
}