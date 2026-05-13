using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.REQMs
{
    public abstract class SignREQM
    {
        public string ApiSignature { get; set; } = null!;
        public long RequestDate { get; set; }
    }
}
