using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Common
{
    public class Singleton<T> where T : new()
    {
        public static T Instance
        {
            get { return SingletonCreator.Instance; }
        }

        class SingletonCreator
        {
            static T _instance;

            internal static T Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (typeof(T))
                        {
                            if (_instance == null)
                            {
                                _instance = new T();
                            }
                        }
                    }
                    return _instance;
                }
            }
        }
    }
}
