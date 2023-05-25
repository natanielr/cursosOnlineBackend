using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation.AspNetCore;
using Aplicacion.Cursos;
using WebAPI.middleware;
using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Aplicacion.Contratos;
using Seguridad.TokenSeguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructores;
using Microsoft.OpenApi.Models;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } // esta variable tiene el metodo para accerder a los archivos de configuracion app.settings.json

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // agregar el cors del servidor 
            services.AddCors( e=> e.AddPolicy("corsApp", builder => {
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            }));


            // aqui se debe agregar el servicio que va ser el contexto de la conexion a la bd 
            services.AddDbContext<CursosOnLineContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // agregar la conexion de la bd con dapper para poder usar procedimientos almacenados
            services.AddOptions();
            services.Configure<ConexionConfig>(Configuration.GetSection("ConnectionStrings")); // asigna una propiedad del archivo json
            // que por dentro tiene una propiedad con el valor de la cadena de conexion.
            // luego crea una clase que tenga como propiedad el nombre de esa propiedad que tiene la conexion para
            // usar esa conexion con dapper y poder invocar los procedimientos almacenados. 
            // En este caso la propiedad que tiene la conexion a la bd es DefaultConnection
            services.AddMediatR(typeof(Consulta.Handler).Assembly);
            services.AddControllers(opt =>
            {
                // agregar validacion global para todos los servicios, es obligado que envien el token por el header de autorizacion
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<NuevoCurso>());

            // implementar identityCore en el webAPI
            var builder = services.AddIdentityCore<Usuario>(opt =>
            {
                // opt.Password.RequireDigit = false;
            });
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            // agregar servicios para instanciar el role manager y agregar los roles al token
            identityBuilder.AddRoles<IdentityRole>();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Usuario, IdentityRole>>();

            // agregar servicio para que levante la instancia de la bd 
            identityBuilder.AddEntityFrameworkStores<CursosOnLineContext>();
            // decirle a netcore que quien va manejar el login y toda la seguridad es core identity
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.TryAddSingleton<ISystemClock, SystemClock>(); // se agrega esta linea para generar los usuarios de identity core


            // agregar validacion de token para las rutas
            var key = SecretKey.getSecretKey();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // si la firma es correcta
                    IssuerSigningKey = key,
                    // limitar el envio de tokens para ciertos criterios como las ips, para limitar el acceso de la aplicacion
                    // por ahora la aplicacion sera global
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });
            /*  
                mapear repositorios e interfaz que implementan funcionalidad en los controladores 
            */
            services.AddScoped<IJwtGenerador, JwtGenerador>();
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();
            //inyctar el servicio para el automapeado de los dtos
            services.AddAutoMapper(typeof(Consulta.Handler));

            // inyectar servicio para usar dapper 

            services.AddTransient<IFactoryConnection, FactoryConnection>();
            // agregar la inyeccion de dependencia
            /*
                Cada vez que se crea una implementacion de tipo repository se debe invocar aqui
            */
            services.AddScoped<IInstructor, InstructorRepository>();
            services.AddScoped<IPaginacion, PaginacionRepository>();

            // agregar metodo para soportar el swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Servicios para mantenimiento de cursos",
                    Version = "v1"
                });

                c.CustomSchemaIds(c => c.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // usar la poliza de cors 

            app.UseCors("corsApp");

            app.UseMiddleware<Error>(); // habilitar el middleware para mostrar los mensajes personalizados al cliente
            // se debe llamar la clase que contenga el middleware que se va ejecutar

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection(); usar esta opcion cuando la aplicacion se sube a produccion. Porque se va usar un certificado de seguridad
            // para navegar usado https

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Habilitar interfaz grafica de swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursos OnLine");
            });
        }
    }
}
