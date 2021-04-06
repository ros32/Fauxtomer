using Fauxtomer.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Services
{
    public class PersonGenerator : IPersonGenerator
    {
        private List<NameRow> _maleNames;
        private List<NameRow> _femaleNames;
        private List<NameRow> _lastNames;
        private List<PersonalNumber> _malePersonalNumbers;
        private List<PersonalNumber> _femalePersonalNumbers;
        private int _medianMaleNameWeight;
        private int _medianFemaleNameWeight;
        private int _medianLastNameWeight;
        public List<Person> DefaultPersons { get; private set; }

        public PersonGenerator()
        {
            Initialize();
        }

        public Person GeneratePerson(int id)
        {
            if (DefaultPersons != null && id < 1000 && DefaultPersons.Count >= id)
                return DefaultPersons[id];
            var rnd = new Random(id);
            var isMale = rnd.Next(2) == 1;
            var firstName = GetName(isMale ? NameRow.NameType.Male : NameRow.NameType.Female, rnd);
            var lastName = GetName(NameRow.NameType.LastName, rnd);
            PersonalNumber personalNumber;
            if (isMale)
            {
                personalNumber = _malePersonalNumbers[rnd.Next(_malePersonalNumbers.Count)];
            }
            else
            {
                personalNumber = _femalePersonalNumbers[rnd.Next(_femalePersonalNumbers.Count)];
            }

            var emailAddress = RemoveDiacritics($"{firstName}.{lastName}{rnd.Next(99)}@example.invalid").ToLower();
            var phoneNumber = $"+4600{rnd.Next(1000000, 9999999)}";
            return new Person
            {
                Id = id,
                PersonalNumber = $"{personalNumber.Identifier[2..8]}-{personalNumber.Identifier[8..12]}",
                FirstName = firstName,
                LastName = lastName,
                AssociatedAddresses = new List<Address>()
                {
                    GetStreetAddress(rnd, new Random((int)(id * Math.PI)), true, 1)
                },
                CurrentAddressId = 1,
                EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Id = 1,
                        Address = emailAddress,
                        CreationDate = DateTime.Now,
                        Validated = true
                    }
                },
                PreferredEmailAddressId = 1,
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Id = 1,
                        Number = phoneNumber,
                        IsMobile = true,
                        Validated = true,
                        CreationDate = DateTime.Now
                    }
                },
                PreferredMobilePhoneNumberId = 1,
            };
        }

        private Address GetStreetAddress(Random rnd, Random rnd2, bool isPrimary, int id)
        {
            var hasCo = rnd.Next(20) % 19 == 0;
            var isMale = rnd2.Next(1) == 0;
            var co = hasCo ? $"c/o {GetName(isMale ? NameRow.NameType.Male : NameRow.NameType.Female, rnd2)} {GetName(NameRow.NameType.LastName, rnd2)}" : null;
            return new Address()
            {
                Id = id,
                CareOfAddress = co,
                StreetAddress = $"{StreetAddress[rnd.Next(StreetAddress.Count)]} {rnd.Next(1, 32)}{((rnd.Next(2) == 1) ? " lgh 1" + rnd.Next(1, 6) + "0" + rnd.Next(2, 10) : "")}",
                PostalCode = $"999 {rnd.Next(11, 100)}",
                City = Cities[rnd.Next(Cities.Count)],
                Country = "SWEDEN"
            };
        }

        private string GetName(NameRow.NameType type, Random rnd)
        {
            var list = (type == NameRow.NameType.Female
                ? _femaleNames
                : type == NameRow.NameType.Male
                    ? _maleNames
                    : _lastNames).OrderBy(p => p.Weight).ToList();
            if (rnd.Next(5) % 4 == 0)
                return list[rnd.Next(list.Count)].Name;
            var value = rnd.NextDouble() * list[^1].Weight;
            if (value < list.Min(p => p.Weight))
                value = list.Min(p => p.Weight) + 1;
            return list.Last(name => name.Weight < value).Name;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void Initialize()
        {
            var maleNamesFile = File.ReadAllText("./Data/Json/MaleNames.json");
            _maleNames = JsonSerializer.Deserialize<List<NameRow>>(maleNamesFile);
            var femaleNamesFile = File.ReadAllText("./Data/Json/FemaleNames.json");
            _femaleNames = JsonSerializer.Deserialize<List<NameRow>>(femaleNamesFile);
            var lastNamesFile = File.ReadAllText("./Data/Json/LastNames.json");
            _lastNames = JsonSerializer.Deserialize<List<NameRow>>(lastNamesFile);
            var personalNumbersFile = File.ReadAllText("./Data/Json/PersonalNumbers.json");
            var personalNumbers = JsonSerializer.Deserialize<List<PersonalNumber>>(personalNumbersFile);
            _malePersonalNumbers = personalNumbers?.Where(p => p.Identifier[10] % 2 == 0).ToList();
            _femalePersonalNumbers = personalNumbers?.Where(p => p.Identifier[10] % 2 == 1).ToList();

           
            _medianMaleNameWeight = _maleNames.Count > 1 
                ? _maleNames.OrderBy(p => p.Weight).ToList()[_maleNames.Count / 2 - 1].Weight
                : 1;
            _medianFemaleNameWeight = _femaleNames.Count > 1
                ? _femaleNames.OrderBy(p => p.Weight).ToList()[_femaleNames.Count / 2 - 1].Weight
                : 1;
            _medianLastNameWeight = _lastNames.Count > 1 
                ? _lastNames.OrderBy(p => p.Weight).ToList()[_femaleNames.Count / 2 - 1].Weight
                : 1;

            var defaultPersons = new List<Person>();
            for (var i = 0; i < 1000; i++)
            {
                defaultPersons.Add(GeneratePerson(i));
            }
            DefaultPersons = defaultPersons;
        }

        private List<string> Cities => new List<string>()
        {
            "TESTSTADEN",
            "TESTBERGA",
            "TESTHAMRA",
            "TESTINGE",
            "TESTEBORG",
            "TESTKÖPING",
            "TESTMORA",
            "TESTVIDABERG",
            "TESTBY",
            "TESTLINGE",
            "TESTÅ",
            "TESTÖN"
        };

        private List<string> StreetAddress => new List<string>()
        {
            "Testgatan",
            "Testvägen",
            "Teststigen",
            "Test Testssons Allé",
            "Södra Testvägen",
            "Norra Testvägen",
            "Övre Testvägen",
            "Nedre Testvägen",
            "Testgränd",
        };
    }
}
