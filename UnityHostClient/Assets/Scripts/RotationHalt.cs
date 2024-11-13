using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHalt : MonoBehaviour
{
    public Transform parentTransform;
    public float yOffset = -38f;

    void Update() {
        if (parentTransform != null) {
            transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y + yOffset, parentTransform.position.z);

            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
