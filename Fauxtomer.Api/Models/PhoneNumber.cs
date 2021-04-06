using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class PhoneNumber : IAnonymizable
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Number { get; set; }
        [JsonIgnore]
        public string NumberHash { get; set; }
        public bool Validated { get; set; }
        public bool IsMobile { get; set; }
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
            this.Number = null;
            this.AnonymizationDate = DateTime.Now;
            this.IsAnonymized = true;
        }
    }
}
