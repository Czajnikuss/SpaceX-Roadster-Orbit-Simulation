using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moveBackCamera : MonoBehaviour
{
    private Vector3 startPoint;
    private Quaternion startRotation;
    
    
    void Start()
    {
        startPoint = transform.position;
        startRotation = transform.rotation;
    }

    
    public void ResetCamera()
    {
        transform.position = startPoint;
        transform.rotation = startRotation;
    }
}
