using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web.Hosting;

namespace PermaisuriCMS.Common
{
    public static class CacheHelper
    {
// ReSharper disable FieldCanBeMadeReadOnly.Local
        private static Cache _cache;
// ReSharper restore FieldCanBeMadeReadOnly.Local

        public static double SaveTime
        {
            get { return _saveTime; }
            set { _saveTime = value; }
        }


        private static double _saveTime = 1;

        static CacheHelper()
        {
            _cache = HostingEnvironment.Cache;
            SaveTime = 15.0;
        }

        public static object Get(string key)
        {
            return string.IsNullOrEmpty(key) ? null : _cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            var obj = Get(key);
            return obj == null ? default(T) : (T) obj;
        }

        public static void Insert(string key, object value, CacheDependency dependency = null,
            CacheItemPriority priority = CacheItemPriority.Default, CacheItemRemovedCallback callback = null)
        {
            _cache.Insert(key, value, dependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SaveTime), priority,
                callback);
        }



        public static void Insert(string key, object value, CacheDependency dependency,
            CacheItemRemovedCallback callback)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, callback);
        }

        public static void Insert(string key, object value, CacheDependency dependency)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, null);
        }

        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _cache.Remove(key);
        }

        public static IEnumerable<string> GetKeys()
        {
            var keys = new List<string>();
            var enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            return keys.AsReadOnly();
        }

        public static void RemoveAll()
        {
            var keys = GetKeys();
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
        }
    }
}
