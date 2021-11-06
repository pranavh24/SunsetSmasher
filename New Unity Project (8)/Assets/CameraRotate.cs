using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float speed;
    
    void Update()
    {
        if (transform.localEulerAngles.y < 80 || transform.localEulerAngles.y > 100) speed *= -1; 
        transform.Rotate(0, speed*Time.deltaTime, 0);
    }
}
