using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WebSocketManagerTankAim : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    [SerializeField] private InputActionReference moveActionToUse;
    
    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null) {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
    }

    void Update() {
        
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"aim {moveDirection.x} {moveDirection.y}");
    }

    public void FireControl()
    {
        webSocketRouter.SendInput("fire");
    }
}