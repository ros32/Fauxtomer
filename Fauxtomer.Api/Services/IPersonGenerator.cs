using Fauxtomer.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Services
{
    public interface IPersonGenerator
    {
        List<Person> DefaultPersons { get; }
        List<Person> DefaultTestPersons { get; }
        Person GeneratePerson(int id, bool nonPlausableNames = false);
    }
}
