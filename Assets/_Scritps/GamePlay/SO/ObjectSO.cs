using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectSO", menuName = "ScriptableObjects/ObjectSO")]
public class ObjectSO : ScriptableObject
{
    public string objectName;

    public int maxHealth = 100;
    public float linearDamping = 5f;

    [Header("How to Interact")]
    public Sprite targetIcon;
    public string textTutorial;

    [Header("Nguyên Liệu rơi ra")]
    public MaterialSO materialSO;
}
