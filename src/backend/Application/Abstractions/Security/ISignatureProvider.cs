using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Security
{
    public interface ISignatureProvider
    {
        string Generate(long requestDate);

        bool Validate(long requestDate, string signature);

        bool IsFresh(long requestDate);
    }
}
