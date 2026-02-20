using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAcceleration : MonoBehaviour
{
    [SerializeField] private ObstacleMetadata metadata;
    [SerializeField] private float startSpeed = 10;
    [SerializeField] private float finalSpeed = 20;
    [SerializeField] private float accelerationTime = 30;
    
    private float startTime;

    void Start()
    {
        
    }

    public void Restart()
    {
        metadata.obstacleSpeed = startSpeed;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        metadata.obstacleSpeed = Mathf.Lerp(startSpeed, finalSpeed, (Time.time - startTime) / accelerationTime);
    }
}
