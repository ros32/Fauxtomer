using Fauxtomer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Services
{
    public class PrivateCustomerService : IPrivateCustomerService
    {
        private IPersonGenerator _generator;

        public PrivateCustomerService(IPersonGenerator generator)
        {
            this._generator = generator;
        }

        public List<SimplePrivateCustomer> FindCustomer(string personalNumber = null, string firstName = null, string lastName = null, string city = null, string country = null)
        {
            var allEmpty = string.IsNullOrWhiteSpace(personalNumber) && string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(country);
            var result = (allEmpty ? _generator.DefaultPersons.Skip(1).Take(50) : _generator.DefaultPersons.Skip(1).Where(p =>
            (string.IsNullOrWhiteSpace(personalNumber) || p.PersonalNumber.Equals(personalNumber, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(firstName) || p.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(lastName) || p.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(city) || (p.CurrentAddress?.City.Equals(city, StringComparison.InvariantCultureIgnoreCase) ?? false))
            && (string.IsNullOrWhiteSpace(country) || (p.CurrentAddress?.Country.Equals(country, StringComparison.InvariantCultureIgnoreCase) ?? false))))
                .Select(p => new SimplePrivateCustomer()
                {
                    Id = p.Id,
                    PersonalNumber = p.PersonalNumber,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    City = p.CurrentAddress?.City,
                    Country = p.CurrentAddress?.Country
                })
                //.OrderByDescending(p => (p.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase) && p.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)))
                //.ThenByDescending(p => p.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
                //.ThenByDescending(p => p.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            return result;
        }

        public Person GetCustomer(int id)
        {
            var person = _generator.GeneratePerson(id);
            return person;
        }
    }
}
