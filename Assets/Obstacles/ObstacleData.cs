using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObstacleData : ScriptableObject
{
    [Range(0, 5)]
    public int damage;

    [Range(0, 20)]
    public float height;

    public ObstacleMetadata meta;
}
