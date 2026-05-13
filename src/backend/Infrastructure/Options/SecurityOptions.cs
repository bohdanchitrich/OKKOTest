using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Options
{
    internal sealed class SecurityOptions
    {
        public string StaticKey { get; init; } = default!;
        public int SimpleTokenLifetimeMinutes { get; init; }
        public int SignatureLifetimeMinutes { get; init; }
        public int FullTokenLifetimeHours { get; init; }
    }
}
