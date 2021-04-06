using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class PersonalNumber
    {
        public string Identifier { get; set; }
        public PersonalNumberType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PersonalNumberType
        {
            Unknown = 0,
            PersonalNumber = 1
        }
    }
}
