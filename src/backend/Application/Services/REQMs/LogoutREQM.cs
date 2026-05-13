using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.REQMs
{
    public class LogoutREQM : SignREQM
    {
        public string FullToken { get; set; } = null!;
    }
}
