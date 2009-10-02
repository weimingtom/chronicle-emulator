using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chronicle.Utility
{
    public static class Reflector
    {
        public static List<Doublet<T1, T2>> FindAllMethods<T1, T2>()
            where T1 : Attribute
            where T2 : class
        {
            if (!typeof(T2).IsSubclassOf(typeof(Delegate))) return null;
            List<Doublet<T1, T2>> results = new List<Doublet<T1, T2>>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    MethodInfo[] methods = type.GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        T1 attribute = Attribute.GetCustomAttribute(method, typeof(T1), false) as T1;
                        if (attribute == null) continue;
                        T2 callback = Delegate.CreateDelegate(typeof(T2), method, false) as T2;
                        if (callback == null) continue;
                        results.Add(new Doublet<T1, T2>(attribute, callback));
                    }
                }
            }
            return results;
        }

        /*
        public static List<Type> FindAllClasses<T>()
            where T : class, new()
        {
            List<Type> results = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                Type typeT = typeof(T);
                Array.ForEach(assembly.GetTypes(), t => { if (t.IsSubclassOf(typeT)) results.Add(t); });
            }
            return results;
        }
        */
    }
}
