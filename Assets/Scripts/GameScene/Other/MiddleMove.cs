using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleMove : MonoBehaviour
{
    float dir;

    void Start()
    {
        if(Random.Range(0f, 1f) > 0.5) {
            dir = -1f;
        } else dir = 1f;
    }

    void Update()
    {
        if(!Game.inGame) return;
        float zRot = transform.rotation.eulerAngles.z + dir;
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f, zRot));
    }
}
