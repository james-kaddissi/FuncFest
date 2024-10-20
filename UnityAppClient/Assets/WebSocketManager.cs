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
        SendInput($"aim {moveDirection.x} {moveDirection.y}");
    }

    public void FireControl()
    {
        SendInput("fire");
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }
}