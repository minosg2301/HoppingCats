using System;
using System.Collections.Generic;
using ERandom = UnityEngine.Random;

public static class DictionaryExt
{
    /// <summary>
    /// Get Or Create new value with key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static V GetOrCreate<K, V>(this Dictionary<K, V> map, K k, Func<K, V> creator = null)
    {
        if(k == null) return default;
        if(!map.ContainsKey(k))
        {
            if(creator != null) map[k] = creator(k);
            else map[k] = (V)Activator.CreateInstance(typeof(V));
        }
        return map[k];
    }

    /// <summary>
    /// Get Or Create new value with key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static V GetOrCreate<K, V>(this Dictionary<K, V> map, K k, Func<V> creator = null)
    {
        if(k == null) return default;
        if(!map.ContainsKey(k))
        {
            if(creator != null) map[k] = creator();
            else map[k] = (V)Activator.CreateInstance(typeof(V));
        }
        return map[k];
    }

    /// <summary>
    ///
    /// </summary>
    public static Dictionary<K, V> Filter<K, V>(this Dictionary<K, V> map, Predicate<K> selector)
    {
        Dictionary<K, V> newMap = new Dictionary<K, V>();
        foreach(var pair in map)
            if(selector.Invoke(pair.Key)) newMap[pair.Key] = pair.Value;
        return newMap;
    }
}