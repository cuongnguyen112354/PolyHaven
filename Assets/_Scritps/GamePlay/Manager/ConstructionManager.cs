using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance;

    [Header("----------Materials----------")]
    public Material validMat;           // Vật liệu hợp lệ
    public Material invalidMat;         // Vật liệu không hợp lệ

    private readonly Dictionary<string, List<GameObject>> placedObjects = new(); 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RemovePlacedObject(string itemName, GameObject obj)
    {
        if (placedObjects.ContainsKey(itemName))
        {
            placedObjects[itemName].Remove(obj);
            Destroy(obj);
        }
    }

    public void AddPlacedObject(string itemName, Vector3 position, Quaternion rotation)
    {
        if (!placedObjects.ContainsKey(itemName))
            placedObjects[itemName] = new List<GameObject>();

        GameObject prefab = Resources.Load<GameObject>($"_Models/Retrievables/{itemName}");
        GameObject obj = Instantiate(prefab);

        obj.name = itemName;
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(transform);

        placedObjects[itemName].Add(obj);
    }

    public void Init(List<ObjectData> objectDatas)
    {
        foreach (ObjectData data in objectDatas)
        {
            AddPlacedObject(data.itemName, data.position, data.rotation);
        }
    }

    public List<ObjectData> GetConstructionData()
    {
        List<ObjectData> objectDatas = new();
        foreach (var kvp in placedObjects)
        {
            string itemName = kvp.Key;
            foreach (var obj in kvp.Value)
            {
                objectDatas.Add(new ObjectData
                {
                    itemName = itemName,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation
                });
            }
        }
        return objectDatas;
    }

    internal void Init(object v)
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class ObjectData
{
    public string itemName;
    public Vector3 position;
    public Quaternion rotation;

    public ObjectData()
    {
        itemName = "Campfire";
        position = new Vector3( (float) -76.5999984741211, (float) 1.2160568237304688, (float) -63.5);
        rotation = new Quaternion(0, 0, 0, 1);
    }
}