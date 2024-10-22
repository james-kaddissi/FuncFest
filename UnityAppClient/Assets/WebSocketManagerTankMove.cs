using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WebSocketManagerTankMove : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionToUse;
    private WebSocketRouter webSocketRouter;

    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null) {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
    }

    void Update() {
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"move {moveDirection.x} {moveDirection.y}");
    }

    public void ReverseControl()
    {
        webSocketRouter.SendInput("reverse");
    }
}