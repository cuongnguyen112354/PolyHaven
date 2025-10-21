using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    private readonly Dictionary<string, List<GameObject>> objectsInEnvironment = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RemoveObject(string itemName, GameObject obj)
    {
        if (objectsInEnvironment.ContainsKey(itemName))
        {
            objectsInEnvironment[itemName].Remove(obj);
            Destroy(obj);
        }
    }

    public void AddObject(string itemName, Vector3 position)
    {
        if (!objectsInEnvironment.ContainsKey(itemName))
            objectsInEnvironment[itemName] = new List<GameObject>();

        GameObject prefab = Resources.Load<GameObject>($"_Models/Environment Objects/{itemName}");
        GameObject obj = Instantiate(prefab);

        obj.name = itemName;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(transform);

        objectsInEnvironment[itemName].Add(obj);
    }

    public void AddObject(string itemName, Vector3 position, Quaternion rotation)
    {
        if (!objectsInEnvironment.ContainsKey(itemName))
            objectsInEnvironment[itemName] = new List<GameObject>();

        GameObject prefab = Resources.Load<GameObject>($"_Models/Environment Objects/{itemName}");
        GameObject obj = Instantiate(prefab);

        obj.name = itemName;
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(transform);

        objectsInEnvironment[itemName].Add(obj);
    }

    public void Init(List<EnvironmentObject> objectDatas)
    {
        foreach (EnvironmentObject data in objectDatas)
        {
            AddObject(data.objectName, data.position);
        }
    }

    public List<EnvironmentObject> GetEnvironmentObject()
    {
        List<EnvironmentObject> objectDatas = new();
        foreach (var kvp in objectsInEnvironment)
        {
            string objectName = kvp.Key;
            foreach (var obj in kvp.Value)
            {
                objectDatas.Add(new EnvironmentObject
                {
                    objectName = objectName,
                    position = obj.transform.position,
                });
            }
        }
        return objectDatas;
    }
}

[System.Serializable]
public class EnvironmentObject
{
    public string objectName;
    public Vector3 position;
}