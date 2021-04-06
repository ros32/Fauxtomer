using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fauxtomer.Api.Models
{
    public interface IAnonymizable
    {
        bool IsAnonymized { get; }
        DateTime? AnonymizationDate { get; }
        void Anonymize();
    }
}
