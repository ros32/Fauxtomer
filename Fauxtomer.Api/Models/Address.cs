using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class Address : IAnonymizable
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string CareOfAddress { get; set; }
        public string StreetAddress { get; set; }

        [JsonIgnore]
        public string Hash { get; set; }

        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [JsonIgnore]
        public DateTime CreationDate { get; set; }
        [JsonIgnore]
        public DateTime? ActivationDate { get; set; }
        [JsonIgnore]
        public DateTime? DeactivationDate { get; set; }

        public bool IsAnonymized { get; private set; }

        [JsonIgnore]
        public DateTime? AnonymizationDate { get; private set; }

        public void Anonymize()
        {
            this.StreetAddress = null;
            this.CareOfAddress = null;
            this.PostalCode = null;
            this.City = null;
            this.Country = null;
            this.AnonymizationDate = DateTime.Now;
            this.IsAnonymized = true;
        }
    }
}
