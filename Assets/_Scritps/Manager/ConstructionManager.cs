using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance;

    [Header("----------Materials----------")]
    public Material validMat;           // Vật liệu hợp lệ
    public Material invalidMat;         // Vật liệu không hợp lệ

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
