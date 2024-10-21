using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManagerRouter : MonoBehaviour
{
    private WebSocket websocket;
    private bool sentUUID = false;

    async void Start() {
        websocket = new WebSocket("ws://192.168.1.156:6789");

        websocket.OnOpen += () => {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) => {
            Debug.Log($"Error: {e}");
        };

        websocket.OnClose += (e) => {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) => {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"Received: {message}");
        };

        await websocket.Connect();
    }

    void Update() {
        if (!sentUUID) {
            Invoke("sendUUID", 1);
            sentUUID = true;
        }
    }

    void sendUUID() {
        string uuid = PlayerPrefs.GetString("DeviceUUID");
        SendInput($"uuid {uuid}");
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }
}
