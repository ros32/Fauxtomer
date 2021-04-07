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

        /// <summary>
        /// Find a customer
        /// </summary>
        /// <param name="personalNumber"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        [HttpGet("find")]
        [ProducesResponseType(typeof(List<SimplePrivateCustomer>), StatusCodes.Status200OK)]
        public IActionResult Find(
            [FromQuery] string personalNumber = null,
            [FromQuery] string firstName = null,
            [FromQuery] string lastName = null,
            [FromQuery] string address = null,
            [FromQuery] string city = null, 
            [FromQuery] string country = null)
        {
            var result = _service.FindCustomer(personalNumber, firstName, lastName, address, city, country) ?? new List<SimplePrivateCustomer>();
            return Ok(result);
        }

        /// <summary>
        /// Get a customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        public IActionResult Get(int id)
        {
            if (id < 1)
                return NoContent();
            var person = _service.GetCustomer(id);
            return Ok(person);
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update()
        {
            return NoContent();
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create()
        {
            return Ok();
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
