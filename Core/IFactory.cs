using System;

namespace HwModule.Core
{
    public interface IFactory<T>
    {
        T Create(params object[] args);
    }

    public class FactoryException : Exception
    {
        public FactoryException(string message) : base(message)
        {
        }
    }
}
