using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSetup : MonoBehaviour
{
    [SerializeField] private RoadData data;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = data.Position;
        transform.localScale = data.Scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
