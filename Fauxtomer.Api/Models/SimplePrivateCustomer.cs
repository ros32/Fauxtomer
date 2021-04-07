using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class SimplePrivateCustomer
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        [JsonIgnore]
        public string City { get; set; }
        [JsonIgnore]
        public string Country { get; set; }
    }
}
