using JoaoMilone.Pooler.Controller;
using UnityEngine;

public class ChoppableObject : ObjectLife
{
    [SerializeField] private Transform[] spawnPoints;

    private int currenthealth;
    private Animator animator;

    void Start()
    {
        currenthealth = objectSO.maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public override void Affected(int damage)
    {
        currenthealth -= damage;
        if (currenthealth > 0)
        {
            AudioManager.Instance.PlayAudioClip("hit_tree");
            animator.Play("Hitted", 0, 0f);
        }
        else
        {
            Die();
        }
    }

    public override void Die()
    {
        AudioManager.Instance.StopSFXSound();
        AudioManager.Instance.PlayAudioClip("cracking");
        foreach (Transform transform in spawnPoints)
        {
            GameObject wood = ObjectPooler.ME.RequestObject("wood", transform.position, Quaternion.Euler(transform.rotation.eulerAngles));

            if (!wood.GetComponent<Rigidbody>())
            {
                wood.AddComponent<Rigidbody>().linearDamping = objectSO.linearDamping;
            }
        }
        
        Destroy(transform.parent.gameObject);
    }
}
