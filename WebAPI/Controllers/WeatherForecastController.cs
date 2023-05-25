using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")] // este route con "[controller]" al ser llamadado desde una peticion http lo que hace es reemplazar el
    // "[controller]" por el nombre de la clase que en este caso es WeatherForecast

    // Por lo tanto para esta ruta la navegacion seria http://localhost:5000/WeatherForecast
    public class WeatherForecastController : ControllerBase
    {

        private readonly CursosOnLineContext context;
        public WeatherForecastController(CursosOnLineContext _context) {
            // agregar la inyeccion de depdencia para usar el contexto que le permite consumir los servicios de la bd
            this.context = _context;
        }
       [HttpGet] // indicar el tipo de verbo http por donde se consume el servicio usar el endpoint
       public IEnumerable<Curso> Get() {
        return this.context.Curso.ToList();
       }
    }
}
