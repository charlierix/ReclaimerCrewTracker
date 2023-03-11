using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReclaimerCrewTracker.models
{
    public record TimeFromTo
    {
        public DateTime From { get; init; }
        public DateTime To { get; init; }
    }
}
