using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebSocketManagerKnockout : MonoBehaviour
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
        webSocketRouter.SendInput($"aim {moveDirection.x} {moveDirection.y}");
        webSocketRouter.SendInput($"power 20.5");
    }
}
