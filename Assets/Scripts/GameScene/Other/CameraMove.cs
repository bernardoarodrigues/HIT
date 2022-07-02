using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float size = Camera.main.orthographicSize;
        if(Game.inGame && size != 9.5f) {
            Camera.main.orthographicSize = Mathf.Lerp(size, 9.5f, Time.deltaTime * 2f);
        } 
    }
}
