using System.Collections.Generic;
using UnityEngine;

public class EndingTileMove : MonoBehaviour
{
    public GameObject ground;
    public float moveSpeed = 0.5f;
    void Start()
    {
        if(ground is null)
            ground = gameObject;
    }

    void Update()
    {
        ground.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}