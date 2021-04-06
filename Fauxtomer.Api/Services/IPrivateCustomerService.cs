using Fauxtomer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Services
{
    public interface IPrivateCustomerService
    {
        List<SimplePrivateCustomer> FindCustomer(string personalNumber = null, string firstName = null, string lastName = null, string city = null, string country = null);
        Person GetCustomer(int id);
    }
}
