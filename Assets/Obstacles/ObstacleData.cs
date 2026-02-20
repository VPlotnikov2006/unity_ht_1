using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObstacleData : ScriptableObject
{
    [Range(0, 100)]
    public float damage;

    [Range(0, 20)]
    public float height;

    public ObstacleMetadata meta;
}
