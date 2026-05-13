using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Security
{
    public interface ITokenProvider
    {
        SimpleTokenEntity CreateSimpleToken(string userLogin);
        bool ValidateSimpleToken(SimpleTokenEntity token);
        FullTokenEntity CreateFullToken(string userLogin);
        bool ValidateFullToken(FullTokenEntity token);

    }
}
