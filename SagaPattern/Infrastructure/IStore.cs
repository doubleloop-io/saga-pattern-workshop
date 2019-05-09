using System;

namespace SagaPattern.Infrastructure
{
    public interface IStore<T> where T : class
    {
        T Get(Guid id);
        void Save(T item);
    }
}