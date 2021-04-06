using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public class NameRow
    {
        public string Name { get; set; }
        public NameType Type { get; set; }
        public int Weight { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum NameType
        {
            Unknown = 0,
            Male = 1,
            Female = 2,
            LastName = 3
        }
    }
}
