using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RESMs
{
    public class ExchangeTokenRESM
    {
        [AdaptMember("Value")]
        public string FullToken { get; set; } = string.Empty;
    }
}
