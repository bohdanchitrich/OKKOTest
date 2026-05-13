using Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Storage
{
    internal sealed class ApplicationContext
    {
        public ConcurrentDictionary<string, SimpleTokenEntity> SimpleTokens
        { get; } = new();

        public ConcurrentDictionary<string, FullTokenEntity> FullTokens
        { get; } = new();

        public ConcurrentDictionary<string, UserEntity> Users
        { get; private set; } = new();

#if DEBUG
        public ApplicationContext()
        {
            Users["admin"] = new UserEntity
            {
                Login = "admin",
                PasswordHash = new Security.HashProvider().Hash("admin")
            };
        }
#endif

        internal ConcurrentDictionary<string,T> Dictionary<T>()
        {
            if (typeof(T) == typeof(SimpleTokenEntity))
                return SimpleTokens as ConcurrentDictionary<string, T> ?? throw new InvalidOperationException($"Failed to cast to {typeof(T).Name}");
            else if (typeof(T) == typeof(FullTokenEntity))
                return FullTokens as ConcurrentDictionary<string, T> ?? throw new InvalidOperationException($"Failed to cast to {typeof(T).Name}");
            else if (typeof(T) == typeof(UserEntity))
                return Users as ConcurrentDictionary<string, T> ?? throw new InvalidOperationException($"Failed to cast to {typeof(T).Name}");
            else
                throw new InvalidOperationException($"Unsupported type: {typeof(T).Name}");
        }

    }
}
