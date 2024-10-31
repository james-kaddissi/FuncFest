using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

public class WebSocketRouter : MonoBehaviour
{
    private static WebSocketRouter instance;

    private WebSocket websocket;
    private bool sentHost = false;

    public Dictionary<int, string> connectedIDs = new Dictionary<int, string>();

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    string GetIPAddress() {
        string localIP = "";
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        
        return localIP;
    }

    async void Start() {
        string ip = GetIPAddress();
        websocket = new WebSocket($"ws://{ip}:6789");

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
        if (!sentHost) {
            Invoke("sendHost", 1);
            sentHost = true;
        }
    }

    void HandleMessage(string message) {
        if(message.StartsWith("Connected ")) {
            string[] parts = message.Substring(10).Split(' ');
            if(parts.Length == 2) {
                int id = int.Parse(parts[0]);
                string uuid = parts[1];  
                connectedIDs[id] = uuid;
                Debug.Log($"Connected ID: {id}, UUID: {uuid}");
                // RouteToScene("TeamTanks");
            }
            Debug.Log("HERE");
        }
    }

    void sendHost() {
        SendInput($"host");
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }

    public void RouteToScene(string scene) {
        SceneManager.LoadScene(scene);
        SendInput($"scene {scene}");
    }
}
