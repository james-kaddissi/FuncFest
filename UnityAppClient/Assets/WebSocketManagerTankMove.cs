using UnityEngine;
using NativeWebSocket;
using UnityEngine.InputSystem;
using System;

public class WebSocketManagerTankMove : MonoBehaviour
{
    private WebSocket websocket;
    [SerializeField] private InputActionReference moveActionToUse;
    bool sentUUID = false;

    async void Start() {
        websocket = new WebSocket("ws://192.168.1.156:6789");
        await websocket.Connect();
    }
    
    void sendUUID() {
        string uuid = PlayerPrefs.GetString("DeviceUUID");
        Debug.Log(uuid);
        SendInput($"uuid {uuid}");
        sentUUID = true;
    }

    void Update() {
        if (sentUUID) {
            Vector2 moveDirection=moveActionToUse.action.ReadValue<Vector2>();
            SendInput($"move {moveDirection.x} {moveDirection.y}");
        } else {
            Invoke("sendUUID", 1);
        }
    }

    public void ReverseControl()
    {
        SendInput("reverse");
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }
}