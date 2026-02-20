using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private RoadData roadData;
    [SerializeField] private List<ObstacleData> obstacleTypes;

    [SerializeField, Min(0.1f)]
    private float spawnDuration = 1f;

    [SerializeField]
    private float spawnZ = 30f;

    [Range(0f, 1f)]
    [SerializeField]
    private float spawnChancePerLane = 0.5f;

    public UnityEvent<float> ApplyDamage;

    [SerializeField] private Player player;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), 0f, spawnDuration);
    }

    private void Spawn()
    {
        int laneCount = Mathf.Max(1, roadData.lanesCount);

        for (int lane = 0; lane < laneCount; lane++)
        {
            if (Random.value <= spawnChancePerLane)
            {
                CreateObstacle(lane);
            }
        }
    }

    private void CreateObstacle(int lane)
    {
        GameObject obj = pool.Get();
        ObstacleScript script = obj.GetComponent<ObstacleScript>();

        script.ApplyDamage.RemoveAllListeners();
        script.ApplyDamage.AddListener(
            damage => ApplyDamage?.Invoke(damage)
        );

        script.Initialize(
            lane,
            obstacleTypes[Random.Range(0, obstacleTypes.Count)],
            pool,
            spawnZ,
            player
        );
    }
}
