using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HwModule.Core
{
    public static class FactoryManager<T> where T : new()
    {
        public static List<T> Factorys { get; set; } = new List<T>();
        public static T GetFactory()
        {
            var t = Factorys.SingleOrDefault(x => x.GetType() == typeof(T));

            if (t == null)
            {
                var factory = new T();
                Factorys.Add(factory);
                return factory;
            }
            else
                return t;
        }
    }
}
