using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class MiControllerBase : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator mediator => _mediator ?? HttpContext.RequestServices.GetService<IMediator>(); // este es el que se comparte con las 
        // clases que heredan de esta
    }
}