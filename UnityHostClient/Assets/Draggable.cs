using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IDragHandler
{
    private Vector3 offset;
    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;

    }
}
