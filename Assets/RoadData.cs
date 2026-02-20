using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RoadData : ScriptableObject
{
    [SerializeField] private float roadLength;
    [SerializeField] private float roadTail;
    [SerializeField] private float roadWidth;
    
    [Min(1)] public int lanesCount = 3;

    private float X => 0;
    private float Y => -0.5f;
    private float Z => (roadLength - roadTail) / 2;

    private float ScaleX => roadWidth;
    private float ScaleZ => roadLength + roadTail;
    private float ScaleY => 1;

    public Vector3 Position => new Vector3(X, Y, Z);
    public Vector3 Scale => new Vector3(ScaleX, ScaleY, ScaleZ);
}
