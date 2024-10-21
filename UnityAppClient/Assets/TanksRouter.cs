using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.SceneManagement;

public class TanksRouter : MonoBehaviour
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
            processMessage(message);
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

    void processMessage(string message) {
        if(message.StartsWith("Player 99 action: ")) {
            string action = message.Substring(19);
            Debug.Log($"Action: {action}");
            // action = "uuid d/g"
            string[] parts = action.Split(' ');
            if(parts[0] == PlayerPrefs.GetString("DeviceUUID")) {
                // This action is for us
                if(parts[1] == "d") {
                    // assign to driver
                    SceneManager.LoadScene("TankMoveController");
                } else if (parts[1] == "g") {
                    // assign to gunner
                    SceneManager.LoadScene("TankGunController");
                }
            }
        }
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }
}
