using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconMove : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float zRot = transform.rotation.eulerAngles.z - 1f;
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f, zRot));
    }
}
