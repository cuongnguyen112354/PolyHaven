using JoaoMilone.Pooler.Controller;
using UnityEngine;

public class ChoppableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite pickableTargetIcon;
    [SerializeField] private string textTutorial = "[Left Mouse]";

    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float woodDamping = 3f;

    private int hp = 5;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Dead()
    {
        AudioManager.Instance.StopSFXSound();
        AudioManager.Instance.PlayAudioClip("cracking");
        foreach (Transform transform in spawnPoints)
        {
            GameObject wood = ObjectPooler.ME.RequestObject("wood", transform.position, Quaternion.Euler(transform.rotation.eulerAngles));
            
            if (!wood.GetComponent<Rigidbody>())
            {
                wood.AddComponent<Rigidbody>().linearDamping = woodDamping;
            }
        }

        enabled = false;
        GetComponent<Collider>().enabled = false;
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
        if (--hp > 0)
        {
            AudioManager.Instance.PlayAudioClip("hit_tree");
            animator.Play("Hitted", 0, 0f);
        }
        else
        {
            // AudioManager.Instance.PlayAudioClip("cracking");
            // animator.SetTrigger("fall");
            Dead();
        }
    }
}
