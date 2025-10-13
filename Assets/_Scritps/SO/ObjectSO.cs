using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectSO", menuName = "ScriptableObjects/ObjectSO")]
public class ObjectSO : ScriptableObject
{
    public string objectName;

    public int maxHealth = 100;
    public float linearDamping = 5f;

    public Sprite pickableTargetIcon;
    public string textTutorial;
    public string interactSoundKey;
}
