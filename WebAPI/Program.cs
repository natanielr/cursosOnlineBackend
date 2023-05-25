using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var HostServer = CreateHostBuilder(args).Build();

            // se crea un conexto para poder conectarse a la bd y generar las migraciones que van a crear las tablas de sql server
            using (var scope = HostServer.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<Usuario>>();
                    var context = services.GetRequiredService<CursosOnLineContext>();
                    context.Database.Migrate();

                    // agregar un usuario sino existe en la bd
                    DataPrueba.InsertarData(context,userManager).Wait(); // Wait() emula el await de una funcion asincrona 

                }
                catch (System.Exception ex)
                {
                    var Logger = services.GetRequiredService<ILogger<Program>>();
                    Logger.LogError(ex, "Ha ocurrido un error en la migracion");
                }
            }

            // una vez generada la migracion con exito, se levanta el servidor

            HostServer.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
