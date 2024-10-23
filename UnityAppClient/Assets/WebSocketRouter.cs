using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.SceneManagement;
using System.Reflection;

public class WebSocketRouter : MonoBehaviour
{
    private static WebSocketRouter instance;

    private WebSocket websocket;
    private bool sentUUID = false;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    async void Start() {
        websocket = new WebSocket("ws://10.156.97.70:6789");

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
            HandleMessage(message);
            var processors = FindObjectsOfType<MonoBehaviour>();
            foreach (var processor in processors) {
                MethodInfo method = processor.GetType().GetMethod("ProcessMessage", BindingFlags.Public | BindingFlags.Instance);
                if (method != null) {
                    method.Invoke(processor, new object[] { message });
                    Debug.Log("Invoked ProcessMessage");
                }
            }
        };

        await websocket.Connect();
    }

    void Update() {
        websocket?.DispatchMessageQueue();
        if (!sentUUID) {
            Invoke("sendUUID", 1);
            sentUUID = true;
        }
    }

    public void HandleMessage(string message) {
        if (message.StartsWith("Player 99 action: ")) {
            string action = message.Substring(18);
            if (action.StartsWith("scene ")) {
                string sceneName = action.Substring(6);
                RouteToScene(sceneName);
            }
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

    public void RouteToScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}
