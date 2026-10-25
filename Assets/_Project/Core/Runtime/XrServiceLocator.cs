using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation.Core
{
    /// <summary>
    /// Thread-safe, non-allocating runtime service locator for decoupling XR subsystems.
    /// </summary>
    public static class XrServiceLocator
    {
        private static readonly Dictionary<Type, object> s_Services = new Dictionary<Type, object>(16);

        public static void Register<T>(T service) where T : class
        {
            Type serviceType = typeof(T);
            if (s_Services.ContainsKey(serviceType))
            {
                Debug.LogWarning($"[XrServiceLocator] Overwriting existing registration for service type {serviceType.Name}");
                s_Services[serviceType] = service;
            }
            else
            {
                s_Services.Add(serviceType, service);
            }
        }

        public static void Unregister<T>() where T : class
        {
            Type serviceType = typeof(T);
            if (s_Services.ContainsKey(serviceType))
            {
                s_Services.Remove(serviceType);
            }
        }

        public static T Get<T>() where T : class
        {
            Type serviceType = typeof(T);
            if (s_Services.TryGetValue(serviceType, out object service))
            {
                return (T)service;
            }

            Debug.LogError($"[XrServiceLocator] Service of type {serviceType.Name} is not registered.");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            Type serviceType = typeof(T);
            if (s_Services.TryGetValue(serviceType, out object rawService))
            {
                service = (T)rawService;
                return true;
            }

            service = null;
            return false;
        }

        public static void Clear()
        {
            s_Services.Clear();
        }
    }
}
