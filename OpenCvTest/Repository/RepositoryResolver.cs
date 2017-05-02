using System;
using System.Collections.Generic;
using AutomatedRodentTracker.RepositoryInterface;

namespace AutomatedRodentTracker.Repository
{
    public static class RepositoryResolver
    {
        private static Dictionary<Type, Func<object>> _TypeDictionary = new Dictionary<Type, Func<object>>(); 

        public static T Resolve<T>() where T : class
        {
            return _TypeDictionary[typeof(T)]() as T;
        }

        static RepositoryResolver()
        {
            _TypeDictionary.Add(typeof(IRepository), () => new Repository());
        }
    }
}
