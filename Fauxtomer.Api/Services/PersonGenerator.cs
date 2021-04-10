using Fauxtomer.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private List<NameRow> _testLastNames;
        private List<PersonalNumber> _malePersonalNumbers;
        private List<PersonalNumber> _femalePersonalNumbers;
        private int _medianMaleNameWeight;
        private int _medianFemaleNameWeight;
        private int _medianLastNameWeight;
        public List<Person> DefaultPersons { get; private set; }
        public List<Person> DefaultTestPersons { get; private set; }

        public PersonGenerator()
        {
            Initialize();
        }

        public Person GeneratePerson(int id, bool nonPlausableNames = false)
        {
            var source = nonPlausableNames ? DefaultTestPersons : DefaultPersons;
            if (source != null && id < 1000 && source.Count >= id)
                return source[id];
            var rnd = new Random(id);
            var isMale = rnd.Next(2) == 1;
            var firstName = GetName(isMale ? NameRow.NameType.Male : NameRow.NameType.Female, rnd);
            var lastName = GetName(NameRow.NameType.LastName, rnd, nonPlausableNames);
            PersonalNumber personalNumber;
            if (isMale)
            {
                personalNumber = _malePersonalNumbers[rnd.Next(_malePersonalNumbers.Count)];
            }
            else
            {
                personalNumber = _femalePersonalNumbers[rnd.Next(_femalePersonalNumbers.Count)];
            }

            var pid = $"{personalNumber.Identifier[2..8]}-{personalNumber.Identifier[8..12]}";
            var person = new Person
            {
                Id = id,
                PersonalNumber = pid,
                FirstName = firstName,
                LastName = lastName,
                AssociatedAddresses = new List<Address>(),
                CurrentAddressId = null,
                EmailAddresses = new List<EmailAddress>(),
                PreferredEmailAddressId = null,
                PhoneNumbers = new List<PhoneNumber>(),
                PreferredMobilePhoneNumberId = null,
                PreferredHomePhoneNumberId = null,
                PersonalNumberHash = ComputeSha256Hash(pid),
            };

            var activeDates = new List<DateTime>()
            {
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-rnd.Next(30, 365)),
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-rnd.Next(1, 30)),
            };

            var emailCount = 1 + (rnd.Next(5) % 4 == 0 ? 1 : 0) + (rnd.Next(10) % 9 == 0 ? 1 : 0);
            var addressCount = 1 + (rnd.Next(15) % 14 == 0 ? 1 : 0) + (rnd.Next(25) % 24 == 0 ? 1 : 0);
            var phoneCount = 0 + (rnd.Next(2) == 1 ? 1 : 0) + (rnd.Next(30) % 29 == 0 ? 1 : 0);
            var mobileCount = 1 + (rnd.Next(4) % 3 == 0 ? 1 : 0) + (rnd.Next(30) % 29 == 0 ? 1 : 0);

            var emailDomains = new Stack<string>(EmailDomains.OrderBy(p => Guid.NewGuid()));
            for (var i = 1; i <= emailCount; i++)
            {
                var domain = emailDomains.Pop();
                var isOld = i < emailCount / 2;
                person.EmailAddresses.Add(GetEmailAddress(rnd, firstName, lastName, domain, true, i, isOld ? activeDates[0] : activeDates[1]));
                if (emailDomains.Count == 0)
                {
                    emailCount = i;
                    break;
                }
            }
            person.PreferredEmailAddressId = rnd.Next(5) % 4 == 0 ? emailCount :  rnd.Next(emailCount) + 1;

            for (var i = 1; i <= addressCount; i++)
            {
                var isOld = i < emailCount / 2;
                person.AssociatedAddresses.Add(GetStreetAddress(rnd, id, i, null, isOld ? activeDates[0] : activeDates[1], nonPlausableNames));
            }
            person.CurrentAddressId = rnd.Next(5) % 4 == 0 ? addressCount : rnd.Next(addressCount) + 1;

            for (var i = 1; i <= phoneCount; i++)
            {
                var isOld = i < emailCount / 2;
                person.PhoneNumbers.Add(GetPhoneNumber(rnd, false, true, i, isOld ? activeDates[0] : activeDates[1]));
            }
            person.PreferredHomePhoneNumberId = phoneCount == 0 ? null : rnd.Next(5) % 4 == 0 ? phoneCount : rnd.Next(phoneCount) + 1;

            for (var i = phoneCount+1; i <= phoneCount+mobileCount; i++)
            {
                var isOld = i < emailCount / 2;
                person.PhoneNumbers.Add(GetPhoneNumber(rnd, true, true, i, isOld ? activeDates[0] : activeDates[1]));
            }
            person.PreferredMobilePhoneNumberId = person.PhoneNumbers.Where(p => p.IsMobile == true).OrderBy(p => rnd.Next(99).ToString()).FirstOrDefault()?.Id;
            
            return person;
        }

        private Address GetStreetAddress(Random rnd, int seed, int id, DateTime? lastDeactivationDate = null, DateTime? deactionvationDate = null, bool nonPlausableNames = false)
        {
            var hasCo = rnd.Next(20) % 19 == 0;
            var co = hasCo ? GeneratePerson(seed * 7, nonPlausableNames) : null;
            var coAddress = hasCo 
                ? co?.CurrentAddress?.CareOfAddress == null 
                    ? $"c/o {co?.FirstName} {co?.LastName}" 
                    : co.CurrentAddress.CareOfAddress 
                : null;
            var address = new Address()
            {
                Id = id,
                CareOfAddress = coAddress,
                StreetAddress = hasCo 
                    ? co.CurrentAddress.StreetAddress 
                    : $"{StreetAddress[rnd.Next(StreetAddress.Count)]} {rnd.Next(1, 32)}{((rnd.Next(2) == 1) ? " lgh 1" + rnd.Next(1, 6) + "0" + rnd.Next(2, 10) : "")}",
                PostalCode = hasCo
                    ? co.CurrentAddress.PostalCode
                    : $"999 {rnd.Next(11, 100)}",
                City = hasCo 
                    ? co.CurrentAddress.City
                    : Cities[rnd.Next(Cities.Count)],
                Country = hasCo 
                    ? co.CurrentAddress.Country
                    : "SWEDEN",
                CreationDate = lastDeactivationDate ?? DateTime.Now,
                ActivationDate = lastDeactivationDate,
                DeactivationDate = deactionvationDate
            };
            return address;
        }

        private PhoneNumber GetPhoneNumber(Random rnd, bool isMobilePhone, bool isValidated, int id, DateTime? activationDate = null)
        {
            var phoneNumber = $"+4600{rnd.Next(1000000, 9999999)}";
            var phone = new PhoneNumber()
            {
                Id = id,
                Number = phoneNumber,
                NumberHash = ComputeSha256Hash(phoneNumber),
                CreationDate = activationDate ?? DateTime.Now,
                IsMobile = isMobilePhone,
                Validated = isValidated,
                ActivationDate = activationDate,
            };
            return phone;
        }

        private EmailAddress GetEmailAddress(Random rnd, string firstName, string lastName, string domain, bool isValidated, int id, DateTime? activationDate = null)
        {
            var emailAddress = RemoveDiacritics($"{firstName}.{lastName}{rnd.Next(99)}{domain}").ToLower();
            emailAddress = emailAddress.Replace("-", "_");
            var email = new EmailAddress()
            {
                Id = id,
                Address = emailAddress,
                Hash = ComputeSha256Hash(emailAddress),
                CreationDate = activationDate ?? DateTime.Now,
                ActivationDate = activationDate,
                Validated = isValidated
            };
            return email;
        }

        private string GetName(NameRow.NameType type, Random rnd, bool nonPlausableName = false)
        {
            var list = (type == NameRow.NameType.Female
                ? _femaleNames
                : type == NameRow.NameType.Male
                    ? _maleNames
                    : nonPlausableName ? _testLastNames : _lastNames).OrderBy(p => p.Weight).ToList();
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
            var testLastNames = File.ReadAllText("./Data/Json/TestLastNames.json");
            _testLastNames = JsonSerializer.Deserialize<List<NameRow>>(testLastNames);
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

            var defaultTestPersons = new List<Person>();
            for (var i = 0; i < 1000; i++)
            {
                defaultTestPersons.Add(GeneratePerson(i, true));
            }
            DefaultTestPersons = defaultTestPersons;
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
            "Testbacken",
        };

        private List<string> EmailDomains => new List<string>()
        {
            "@mailinator.com",
            "@maildrop.cc",
            "@guerrillamail.info"
        };

        static string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrWhiteSpace(rawData))
                return null;
            const string salt = "mskevjskl";
            // Create a SHA256   
            using SHA256 sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(salt + rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
