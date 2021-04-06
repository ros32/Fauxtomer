using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class Person : IAnonymizable
    {
        public int Id { get; set; }
        public string PersonalNumber { get; set; }
        [JsonIgnore]
        public string PersonalNumberHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Address> AssociatedAddresses { get; set; }
        [JsonIgnore]
        public int? CurrentAddressId { get; set; }
        public Address CurrentAddress => AssociatedAddresses?.FirstOrDefault(p => p.Id == CurrentAddressId);
        public List<EmailAddress> EmailAddresses { get; set; }
        [JsonIgnore]
        public int? PreferredEmailAddressId { get; set; }
        public EmailAddress PreferredEmailAddress => EmailAddresses?.FirstOrDefault(p => p.Id == PreferredEmailAddressId);
        public List<PhoneNumber> PhoneNumbers { get; set; }
        [JsonIgnore]
        public int? PreferredMobilePhoneNumberId { get; set; }
        [JsonIgnore]
        public int? PreferredHomePhoneNumberId { get; set; }
        public PhoneNumber PreferredMobilePhoneNumber => PhoneNumbers?.FirstOrDefault(p => p.Id == PreferredMobilePhoneNumberId);
        public PhoneNumber PreferredHomePhoneNumber => PhoneNumbers?.FirstOrDefault(p => p.Id == PreferredHomePhoneNumberId);
        public List<object> AuditLogEntries { get; set; }
        public bool IsAnonymized { get; private set; }

        [JsonIgnore]
        public DateTime? AnonymizationDate { get; private set; }

        public void Anonymize()
        {
            this.PersonalNumber = null;
            this.FirstName = null;
            this.LastName = null;
            AssociatedAddresses?.ForEach(p => p?.Anonymize());
            this.CurrentAddressId = null;
            EmailAddresses?.ForEach(p => p?.Anonymize());
            this.PreferredEmailAddressId = null;
            PhoneNumbers?.ForEach(p => p?.Anonymize());
            this.PreferredMobilePhoneNumberId = null;
            this.PreferredHomePhoneNumberId = null;

            this.IsAnonymized = true;
        }
    }
}
