using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Login { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }
}
