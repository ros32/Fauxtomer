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

        public List<SimplePrivateCustomer> FindCustomer(string personalNumber = null, string firstName = null, string lastName = null, string address = null, string city = null, string country = null, bool nonPlausableNames = false)
        {
            var allEmpty = string.IsNullOrWhiteSpace(personalNumber) 
                && string.IsNullOrWhiteSpace(firstName) 
                && string.IsNullOrWhiteSpace(lastName) 
                && string.IsNullOrWhiteSpace(address) 
                && string.IsNullOrWhiteSpace(city)
                && string.IsNullOrWhiteSpace(country);
            var source = nonPlausableNames ? _generator.DefaultTestPersons : _generator.DefaultPersons;
            var result = (allEmpty ? source.Skip(1).Take(50) : source.Skip(1).Where(p =>
            (string.IsNullOrWhiteSpace(personalNumber) || p.PersonalNumber.Equals(personalNumber, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(firstName) || p.FirstName.Contains(firstName, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(lastName) || p.LastName.Contains(lastName, StringComparison.InvariantCultureIgnoreCase))
            && (string.IsNullOrWhiteSpace(address) || (p.CurrentAddress?.StreetAddress.Contains(address, StringComparison.InvariantCultureIgnoreCase) ?? false))
            && (string.IsNullOrWhiteSpace(city) || (p.CurrentAddress?.City.Contains(city, StringComparison.InvariantCultureIgnoreCase) ?? false))
            && (string.IsNullOrWhiteSpace(country) || (p.CurrentAddress?.Country.Contains(country, StringComparison.InvariantCultureIgnoreCase) ?? false))))
                .Select(p => new SimplePrivateCustomer()
                {
                    Id = p.Id,
                    PersonalNumber = p.PersonalNumber,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Address = $"{(p.CurrentAddress?.CareOfAddress != null ? p.CurrentAddress?.CareOfAddress + ", " : "")}{p.CurrentAddress?.StreetAddress}, {p.CurrentAddress?.PostalCode} {p.CurrentAddress?.City}, {p.CurrentAddress?.Country}",
                    EmailAddress = p.PreferredEmailAddress?.Address,
                    PhoneNumber = p.PreferredHomePhoneNumber?.Number,
                    MobilePhoneNumber = p.PreferredMobilePhoneNumber?.Number,
                    City = p.CurrentAddress?.City,
                    Country = p.CurrentAddress?.Country
                })
                .OrderByDescending(p => (p.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase) && p.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)))
                .ThenByDescending(p => p.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
                .ThenByDescending(p => p.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
                .ThenByDescending(p => !string.IsNullOrWhiteSpace(lastName) && p.LastName.Contains(lastName, StringComparison.InvariantCultureIgnoreCase))
                .ThenByDescending(p => !string.IsNullOrWhiteSpace(firstName) && p.FirstName.Contains(firstName, StringComparison.InvariantCultureIgnoreCase))
                .ThenByDescending(p => !string.IsNullOrWhiteSpace(address) && p.Address.Contains(address, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            return result;
        }

        public Person GetCustomer(int id, bool nonPlausableNames = false)
        {
            var person = _generator.GeneratePerson(id, nonPlausableNames);
            return person;
        }
    }
}
