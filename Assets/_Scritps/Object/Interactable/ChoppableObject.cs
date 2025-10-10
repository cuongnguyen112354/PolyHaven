using UnityEngine;

public class ChoppableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite pickableTargetIcon;
    [SerializeField] private string textTutorial = "[Left Mouse]";

    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private int hp = 5;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void ImpactFXEvent()
    {
        AudioManager.Instance.StopSFXSound();
        AudioManager.Instance.PlayAudioClip("impact_fall");
    }

    void EndFallingEvent()
    {
        foreach (Transform transform in spawnPoints)
            Instantiate(woodPrefab, transform.position, Quaternion.Euler(transform.eulerAngles));
        Destroy(transform.parent.gameObject);
    }

    public string GetItemName()
    {
        return "Tree";
    }

    public (Sprite, string) HowInteract()
    {
        return (pickableTargetIcon, textTutorial);
    }

    public void Interact()
    {
        if (hp > 0)
        {
            AudioManager.Instance.PlayAudioClip("chop");
            animator.SetTrigger("hit");
        }

        if (--hp == 0)
        {
            AudioManager.Instance.PlayAudioClip("cracking");
            animator.SetTrigger("fall");
            enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}
