using Fauxtomer.Api.Models;
using Fauxtomer.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateCustomersController : ControllerBase
    {
        private IPrivateCustomerService _service;

        public PrivateCustomersController(IPrivateCustomerService service)
        {
            this._service = service;
        }

        [HttpGet("find")]
        [ProducesResponseType(typeof(List<SimplePrivateCustomer>), StatusCodes.Status200OK)]
        public IActionResult Find(
            [FromQuery] string personalNumber = null, 
            [FromQuery] string firstName = null, 
            [FromQuery] string lastName = null, 
            [FromQuery] string city = null, 
            [FromQuery] string country = null)
        {
            var result = _service.FindCustomer(personalNumber, firstName, lastName, city, country) ?? new List<SimplePrivateCustomer>();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            if (id < 1)
                return NoContent();
            var person = _service.GetCustomer(id);
            return Ok(person);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update()
        {
            return NoContent();
        }

        [HttpPost]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
