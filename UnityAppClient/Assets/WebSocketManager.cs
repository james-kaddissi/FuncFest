using UnityEngine;
using NativeWebSocket;
using UnityEngine.InputSystem;
using System;

public class WebSocketManager : MonoBehaviour
{
    private WebSocket websocket;
    [SerializeField] private InputActionReference moveActionToUse;

    async void Start() {
        websocket = new WebSocket("ws://192.168.1.156:6789");
        await websocket.Connect();
    }

    void Update() {
        Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
        if(moveDirection != Vector2.zero) {
            SendInput($"move {moveDirection.x} {moveDirection.y}");
        }
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }
}