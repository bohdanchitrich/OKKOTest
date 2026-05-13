using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ConcurrentDictionary<string, T> dictionary;

        public Repository(ApplicationContext context)
        {
            dictionary = context.Dictionary<T>();
        }

        public Task<T> AddAsync(T entity)
        {
            dictionary[entity.Id] = entity;
            return Task.FromResult(entity);
        }

        public Task<List<T>> AddRangeAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                dictionary[entity.Id] = entity;
            }

            return Task.FromResult(entities);
        }

        public Task<bool> DeleteAsync(T entity)
        {
            return Task.FromResult(
                dictionary.Remove(entity.Id, out _)
            );
        }

        public Task<bool> DeleteRangeAsync(List<T> entities)
        {
            bool allRemoved = true;

            foreach (var entity in entities)
            {
                if (!dictionary.Remove(entity.Id, out _))
                {
                    allRemoved = false;
                }
            }

            return Task.FromResult(allRemoved);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();

            var entity = dictionary
                .Values
                .FirstOrDefault(compiledPredicate);

            return Task.FromResult(entity);
        }

        public Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();

            var exists = dictionary
                .Values
                .Any(compiledPredicate);

            return Task.FromResult(exists);
        }

        public Task<T> UpdateAsync(T entity)
        {
            dictionary[entity.Id] = entity;

            return Task.FromResult(entity);
        }

        public Task<List<T>> UpdateRangeAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                dictionary[entity.Id] = entity;
            }

            return Task.FromResult(entities);
        }

        public Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();

            var entities = dictionary
                .Values
                .Where(compiledPredicate)
                .ToList();

            return Task.FromResult(entities);
        }
    }
}
