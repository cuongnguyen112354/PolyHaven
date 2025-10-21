using JoaoMilone.Pooler.Controller;
using UnityEngine;

public class MineableObject : AbObjectLife
{
    public override void Affected(int damage)
    {
        currenthealth -= damage;
        if (currenthealth > 0)
        {
            AudioManager.Instance.PlayAudioClip("mining");
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
        AudioManager.Instance.PlayAudioClip("rock-destroy");

        EnvironmentManager.Instance.RemoveObject(objectSO.objectName, gameObject);

        foreach (Transform transform in spawnPoints)
        {
            GameObject wood = ObjectPooler.ME.RequestObject("stone", transform.position, Quaternion.Euler(transform.rotation.eulerAngles));

            if (!wood.GetComponent<Rigidbody>())
            {
                wood.AddComponent<Rigidbody>().linearDamping = objectSO.linearDamping;
            }
        }

        // Destroy(gameObject);
    }
}
