using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Identity;

namespace Persistencia
{
    public class DataPrueba
    {
        public static async Task InsertarData(CursosOnLineContext context, UserManager<Usuario> userManager) {
            // insertar usuarios de IdentityCore en sql server si no existe ninguno 

            if(!userManager.Users.Any()) {

                var usuario = new Usuario{
                    NombreCompleto="Nataniel Rodriguez",
                    UserName="nrv2391",
                    Email="natanielr@loymark.com"
                };
        
                await userManager.CreateAsync(usuario,"Nvmnataniel1$");
            }
        }
    }
}