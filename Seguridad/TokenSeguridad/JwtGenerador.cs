using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Seguridad.TokenSeguridad
{
    public class JwtGenerador : IJwtGenerador
    {
        public string crearToken(Usuario usuario,List<string> roles)
        {   

            // cuando se crean claims, se debe asginar dos valores, el tipo de claim y el valor del claim
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId,usuario.UserName)
            };

            if(roles != null) {
                foreach(var role in roles){
                    //agregar los claims de roles al token
                    claims.Add(new Claim(ClaimTypes.Role,role));
                }
            }

            // aqui va generar el token de la sesion
                // dejarlo en UTF32 porque da error de que el espcio para el token es muy pequeno
            var key = SecretKey.getSecretKey();
            var credenciales = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescription = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                // agregar la parte de la firma 
                SigningCredentials = credenciales
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token); // devolver el token generado 
        }
    }
}