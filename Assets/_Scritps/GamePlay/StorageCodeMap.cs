using System.Collections.Generic;
using UnityEngine;

public class StorageCodeMap
{
    public static Dictionary<string, Storage> codeMap = new();

    public static void AddCode(string code, Storage component)
    {
        codeMap.TryAdd(code, component);
    }

    public static Storage GetComponentByCode(string code)
    {
        codeMap.TryGetValue(code, out Storage component);
        return component;
    }

    public static void LogAllCode()
    {
        foreach (string key in codeMap.Keys)
        {
            Debug.Log("Key: " + key);
            if (codeMap[key] != null)
                Debug.Log(codeMap[key].gameObject.name);
        }
    }
}
