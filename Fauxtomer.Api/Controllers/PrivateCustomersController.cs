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
            [FromQuery] string country = null,
            [FromQuery] bool returnData = true,
            [FromQuery] bool nonPlausableNames = false)
        {
            var result = _service.FindCustomer(personalNumber, firstName, lastName, address, city, country, nonPlausableNames) ?? new List<SimplePrivateCustomer>();
            return Ok(returnData ? result : result?.Select(p => p.Id).ToArray());
        }

        /// <summary>
        /// Get a customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        public IActionResult Get(int id, [FromQuery] bool nonPlausableNames = false)
        {
            if (id < 1)
                return NoContent();
            var person = _service.GetCustomer(id, nonPlausableNames);
            return Ok(person);
        }
    }
}
