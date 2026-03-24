using UnityEngine;

public class ExtraLives: BonusScript
{
    [Min(0)]
    [SerializeField] private int extraLives;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.ExtraLives(extraLives);
            Destroy(gameObject);
        }
    }

}
