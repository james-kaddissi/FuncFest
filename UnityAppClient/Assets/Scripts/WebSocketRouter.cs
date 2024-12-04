using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;

public class WebSocketRouter : MonoBehaviour
{
    private UdpClient udpListener;
    private string serverIP = "";

    private static WebSocketRouter instance;

    private WebSocket websocket;

    private string playername = "Player";

    public TMP_InputField textInput;

    private bool serverDetected = false;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        udpListener = new UdpClient(6788);
        udpListener.BeginReceive(OnUdpDataReceived, null);
    }

    void Start() {
        string defaultName = PlayerPrefs.GetString("Name");
        if (defaultName != null) {
            textInput.text = defaultName;
            playername = defaultName;
        }
        Debug.Log("default name: " + defaultName);
    }

    private void OnUdpDataReceived(IAsyncResult result)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 6788);
        byte[] data = udpListener.EndReceive(result, ref endPoint);
        string message = System.Text.Encoding.UTF8.GetString(data);
        if (message.StartsWith("Server IP: "))
        {
            serverIP = message.Substring(11); 
            Debug.Log($"Received server IP: {serverIP}");
            if (!serverDetected) {
                serverDetected = true;
                StartConnect();
            }
        }

        udpListener.BeginReceive(OnUdpDataReceived, null);
    }

    string GetIPAddress() {
        return serverIP;
    }

    async void StartConnect() {
        string ip = GetIPAddress();
        websocket = new WebSocket($"ws://{ip}:6789");
        Debug.Log($"Connecting to {ip}:6789");

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


    public void sendUUID() {
        string uuid = PlayerPrefs.GetString("DeviceUUID");
        playername = textInput.text;
        PlayerPrefs.SetString("Name", playername);
        SendInput($"uuid {uuid} {playername}");
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
