using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LitInject
{
    public static class LitInjector
    {
        private static readonly Dictionary<Type, object> _Container = new Dictionary<Type, object>();

        private static readonly Dictionary<Type, List<PropertyInfo>> _CachePropertyInfos1 =
            new Dictionary<Type, List<PropertyInfo>>();
        
        private static readonly Dictionary<Type, List<PropertyInfo>> _CachePropertyInfos2 =
            new Dictionary<Type, List<PropertyInfo>>();

        public static void AddToContainer(Type type, object obj)
        {
            _Container.Add(type, obj);
        }

        public static void InjectDependency(object target, bool declaredOnly = false)
        {
            var cachePropertyInfos = declaredOnly ? _CachePropertyInfos1 : _CachePropertyInfos2;
            
            var type = target.GetType();
            if (cachePropertyInfos.TryGetValue(type, out var properties1))
            {
                foreach (var p in properties1)
                {
                    InternalInjectDependency(target, p);
                }
                return;
            }

            cachePropertyInfos.Add(type, new List<PropertyInfo>());
            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (declaredOnly)
            {
                flags |= BindingFlags.DeclaredOnly;
            }
            var properties2 = type.GetProperties(flags);
            foreach (var p in properties2)
            {
                var attrs = p.GetCustomAttributes(false);
                if (attrs.OfType<InjectAttribute>().Any())
                {
                    InternalInjectDependency(target, p);
                    cachePropertyInfos[type].Add(p);
                }
            }
        }

        private static void InternalInjectDependency(object target, PropertyInfo propertyInfo)
        {
            if (!_Container.TryGetValue(propertyInfo.PropertyType, out var value))
            {
                return;
            }

            propertyInfo.SetValue(target, value);
        }
    }
}