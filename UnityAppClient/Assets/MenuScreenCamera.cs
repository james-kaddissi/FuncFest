using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreenCamera : MonoBehaviour
{
    public float rotationSpeed = 5f; 

    void Update() 
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
