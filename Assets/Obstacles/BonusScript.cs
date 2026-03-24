using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour
{
    [SerializeField] private RoadData road;
    
    public ObstacleMetadata metadata;

    [Min(0)]
    public int row;

    [HideInInspector] public Player player;

    private float[] lanesX;

    public void Initialize(
        int lane,
        float spawnZ,
        Player _player)
    {
        row = lane;
        player = _player;

        InitializeLanes();
        SetupBonus(spawnZ);
    }

    private void InitializeLanes()
    {
        int laneCount = Mathf.Max(1, road.lanesCount);
        lanesX = new float[laneCount];

        float roadWidth = road.Scale.x;
        float laneWidth = roadWidth / laneCount;

        float leftEdge = road.Position.x - roadWidth / 2f;

        for (int i = 0; i < laneCount; i++)
        {
            lanesX[i] = leftEdge + laneWidth * (i + 0.5f);
        }

        row = Mathf.Clamp(row, 0, laneCount - 1);
    }

    private void SetupBonus(float spawnZ)
    {
        transform.position = new Vector3(
            lanesX[row],
            transform.localScale.y / 2f,
            spawnZ
        );
    }

    void Update()
    {
        transform.Translate(
            metadata.obstacleSpeed * Time.deltaTime * Vector3.back
        );

        if (transform.position.z < metadata.deafZ)
        {
            Destroy(gameObject);
        }
    }
}
