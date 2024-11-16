using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class WebSocketManagerTankAim : MonoBehaviour
{
    private WebSocketRouter webSocketRouter;
    [SerializeField] private InputActionReference moveActionToUse;

    private float fireCooldown = 5f;
    private float lastFireTime; 
    [SerializeField] private TextMeshProUGUI fireText;
    
    void Start() {
        webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
        if (webSocketRouter == null) {
            Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
        }
        lastFireTime = -5f;
    }

    void Update() {
        
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        webSocketRouter.SendInput($"aim {moveDirection.x} {moveDirection.y}");

        if (Time.time < lastFireTime + fireCooldown) {
            float remainingTime = lastFireTime + fireCooldown - Time.time;
            fireText.text = $"{Math.Ceiling(remainingTime)}";
        } else {
            fireText.text = "FIRE!";
        }
    }

    public void FireControl()
    {
        if (Time.time >= lastFireTime + fireCooldown) {
            webSocketRouter.SendInput("fire");
            lastFireTime = Time.time; 
        } else {
            Debug.Log("Fire is on cooldown.");
        }
    }
}