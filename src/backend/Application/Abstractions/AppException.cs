using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public abstract class AppException : Exception
    {
        public string Error { get; }

        protected AppException(string error, string message)
            : base(message)
        {
            Error = error;
        }
    }
}
