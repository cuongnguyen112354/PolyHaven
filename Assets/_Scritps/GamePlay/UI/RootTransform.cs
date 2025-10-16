using UnityEngine;

public class RootTransform : MonoBehaviour
{
    public static RootTransform Instance;

    public Transform rootTransform;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
