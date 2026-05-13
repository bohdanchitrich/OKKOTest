using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.RESMs
{
    public class LoginRESM
    {

        [AdaptMember("Value")]
        public string SimpleToken { get; set; } = string.Empty;
    }
}
