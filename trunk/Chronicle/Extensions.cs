using System;
using System.Collections.Generic;

namespace Chronicle
{
    public static class Extensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> pThis, TKey pKey, TValue pDefault)
        {
            TValue result;
            return pThis.TryGetValue(pKey, out result) ? result : pDefault;
        }
    }
}
