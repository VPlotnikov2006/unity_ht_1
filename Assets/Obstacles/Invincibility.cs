using UnityEngine;

public class Invincibility: BonusScript
{
    [Min(0)]
    [SerializeField] private float duration;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.Invincibility(duration);
            Destroy(gameObject);
        }
    }

}
