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

    public GameObject AddPlacedObject(string itemName, Vector3 position, Quaternion rotation)
    {
        if (!placedObjects.ContainsKey(itemName))
            placedObjects[itemName] = new List<GameObject>();

        GameObject prefab = Resources.Load<GameObject>($"_Models/Placeable/{itemName}");
        GameObject obj = Instantiate(prefab);

        obj.name = itemName;
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(transform);

        placedObjects[itemName].Add(obj);

        return obj;
    }

    public void Init(List<ConstructionObject> constructionObjects)
    {
        foreach (ConstructionObject data in constructionObjects)
        {
            AddPlacedObject(data.objectName, data.position, data.rotation);
        }
    }

    public List<ConstructionObject> GetConstructionObject()
    {
        List<ConstructionObject> constructionObjects = new();
        foreach (var kvp in placedObjects)
        {
            foreach (var obj in kvp.Value)
            {
                if (kvp.Key == "Chest") continue;

                constructionObjects.Add(new ConstructionObject
                {
                    objectName = kvp.Key,
                    position = obj.transform.position,
                    rotation = obj.transform.rotation
                });
            }
        }
        return constructionObjects;
    }
}

[System.Serializable]
public class ConstructionObject
{
    public string objectName;
    public Vector3 position;
    public Quaternion rotation;

    public ConstructionObject()
    {
        objectName = "Campfire";
        position = new Vector3( (float) -76.5999984741211, (float) 1.2160568237304688, (float) -63.5);
        rotation = new Quaternion(0, 0, 0, 1);
    }
}