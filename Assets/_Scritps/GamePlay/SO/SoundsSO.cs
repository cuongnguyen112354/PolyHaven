using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "newSounds", menuName = "ScriptableObjects/Sounds")]
public class SoundsSO : ScriptableObject
{
    public List<Sound> sounds;
}

[System.Serializable]
public class Sound
{
    public string key;
    public AudioClip audioClip;
}