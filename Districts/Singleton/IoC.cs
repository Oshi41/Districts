using System;
using System.Collections.Generic;
using Districts.Helper;

namespace Districts.Singleton
{
    public class IoC
    {
        #region Singleton implementation

        
        private static IoC _instance;
        public static IoC Instance => _instance ?? (_instance = new IoC());

        private IoC()
        {
            Register<IMessageHelper>(new MessageHelper());
        }


        #endregion

        #region Fields
        
        private Dictionary<Type, object> _map = new Dictionary<Type, object>();

        #endregion

        public void Register<T>(T instance)
        {
            var type = typeof(T);

            if (_map.ContainsKey(type)
                && !Equals(_map[type], instance))
            {
                throw new Exception($"Can't register type {type} twice");
            }

            _map[type] = instance;
        }

        public T Get<T>()
            where T : class
        {
            var type = typeof(T);
            if (_map.ContainsKey(type))
            {
                return _map[type] as T;
            }

            foreach (var mapKey in _map.Keys)
            {
                if (type.IsAssignableFrom(mapKey)
                    || mapKey.IsSubclassOf(type))
                {
                    return _map[type] as T;
                }
            }


            throw new Exception("Can't find that type!");
        }
    }
}
