using System;

namespace Axtox.IoT.Common.System.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Length == 0)
                return false;

            foreach (var iface in interfaces)
                if (iface == interfaceType)
                    return true;

            return false;
        }
    }
}
