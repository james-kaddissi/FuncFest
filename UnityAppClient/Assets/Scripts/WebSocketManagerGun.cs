using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class WebSocketManagerGun : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private InputActionReference shootActionToUse;
    
    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null) {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
    }

    void Update() {
        
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"move {moveDirection.x} {moveDirection.y}");
        Vector2 shootDirection=shootActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"shoot {shootDirection.x} {shootDirection.y}");
    }

}