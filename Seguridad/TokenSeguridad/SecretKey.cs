using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Seguridad.TokenSeguridad
{
    public class SecretKey
    {
        public static SymmetricSecurityKey getSecretKey() {
            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes("Secretkey"));
            return key;
        }
    }
}