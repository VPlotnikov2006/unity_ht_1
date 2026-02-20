using UnityEngine;
using UnityEngine.Events;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private RoadData road;

    public UnityEvent<float> ApplyDamage;
    public ObstacleData data;

    [Min(0)]
    public int row;

    [HideInInspector] public Player player;

    private ObjectPool pool;
    private float[] lanesX;

    public void Initialize(
        int lane,
        ObstacleData obstacleData,
        ObjectPool objectPool,
        float spawnZ,
        Player _player)
    {
        pool = objectPool;
        data = obstacleData;
        row = lane;
        player = _player;

        InitializeLanes();
        SetupObstacle(spawnZ);
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

    private void SetupObstacle(float spawnZ)
    {
        int laneCount = lanesX.Length;
        float laneWidth = road.Scale.x / laneCount;

        transform.localScale = new Vector3(
            laneWidth,
            data.height,
            transform.localScale.z
        );

        transform.position = new Vector3(
            lanesX[row],
            data.height / 2f,
            spawnZ
        );

        player.OnRestart.AddListener(ReturnToPool);
    }

    void Update()
    {
        transform.Translate(
            data.meta.obstacleSpeed * Time.deltaTime * Vector3.back
        );

        if (transform.position.z < data.meta.deafZ)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage?.Invoke(data.damage);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (isActiveAndEnabled)
        {
            player.OnRestart.RemoveListener(ReturnToPool);
            pool.Return(gameObject);
        }
    }
}
