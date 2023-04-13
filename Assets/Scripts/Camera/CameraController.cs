using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 position;

    void Awake()
    {
        if(!player)
            player = FindObjectOfType<Player>().transform;
    }
    void Update()
    {
        position = player.position;
        position.z = -10f;
        position.y += 1.5f;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 3f);
    }
}
