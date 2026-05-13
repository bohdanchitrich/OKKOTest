using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.REQMs
{
    //REQM - Request Model
    public class LoginREQM : SignREQM
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
     
    }
}
