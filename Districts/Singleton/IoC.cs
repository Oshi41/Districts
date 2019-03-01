using System;
using System.Collections.Generic;
using Districts.New.Implementation;
using Districts.New.Interfaces;
using Districts.Parser.v2;
using Districts.Settings.v2;

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
            Register<IDialogProvider>(new DialogProvider());
            Register<IWebWorker>(new WebWork());
            Register<IAppSettings>(new AppSettings());

            // Инициализируется после IAppSettings!
            Register<IParser>(new Parser.v2.Parser(Get<IAppSettings>()));
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
