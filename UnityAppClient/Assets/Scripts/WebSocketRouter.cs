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
    private bool readyToConnect = false;
    private bool sentName = false;

    public bool gameEnabled = false;
    public GameObject firstButton;
    
    private enum ConnectionState {
        NoConnection,
        CellularConnection,
        WiFiConnection,
        JoinGame,
        Waiting,
        InGames
    }

    private ConnectionState connectionState;

    public GameObject noConnectionScreen;
    public GameObject cellularConnectionScreen;
    public GameObject wifiConnectionScreen;
    public GameObject joinGameScreen;
    public GameObject waitingScreen;

    private bool inGame = false;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        try {
            udpListener = new UdpClient(6788);
            udpListener.BeginReceive(OnUdpDataReceived, null);
            Debug.Log("UDP Listener successfully started on port 6788.");
        }
        catch (Exception ex) {
            Debug.LogError($"Failed to start UDP Listener on port 6788: {ex.Message}");
        }
    }

    void Start() {
        string defaultName = PlayerPrefs.GetString("Name");
        if (defaultName != null) {
            textInput.text = defaultName;
            playername = defaultName;
        }
        Debug.Log("default name: " + defaultName);
        CheckConnectionType();
    }

    public void EnableGame() {
        gameEnabled = true;
        firstButton.SetActive(false);
    }

    private void OnUdpDataReceived(IAsyncResult result)
    {
        try {
            Debug.Log("Received UDP data");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 6788);
            byte[] data = udpListener.EndReceive(result, ref endPoint);
            string message = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log(message);
            if (message.StartsWith("Server IP: "))
            {
                serverIP = message.Substring(11); 
                Debug.Log($"Received server IP: {serverIP}");
                if (!readyToConnect) {
                    serverDetected = true;
                    StartConnect();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in OnUdpDataReceived: {ex.Message}");
        }
        finally
        {
            try
            {
                udpListener.BeginReceive(OnUdpDataReceived, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error restarting UDP listener: {ex.Message}");
            }
        }
    }

    string GetIPAddress() {
        return serverIP;
    }

    void CheckConnectionType()
    {
        if(inGame) {
            connectionState = ConnectionState.InGames;
            return;
        }
        NetworkReachability reachability = Application.internetReachability;

        if (reachability == NetworkReachability.NotReachable)
        {
            connectionState = ConnectionState.NoConnection;
        }
        else if (reachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            connectionState = ConnectionState.CellularConnection;
        }
        else if (reachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            if(sentName){
                connectionState = ConnectionState.Waiting;
                return;
            }
            if(readyToConnect){
                connectionState = ConnectionState.JoinGame;
            } else {
                connectionState = ConnectionState.WiFiConnection;
            }
        }
    }

    async void StartConnect() {
        string ip = GetIPAddress();
        websocket = new WebSocket($"ws://{ip}:6789");
        Debug.Log($"Connecting to {ip}:6789");

        websocket.OnOpen += () => {
            Debug.Log("Connection open!");
            readyToConnect = true;
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
        if (gameEnabled) {
            switch(connectionState) {
                case ConnectionState.NoConnection:
                    noConnectionScreen.SetActive(true);
                    cellularConnectionScreen.SetActive(false);
                    wifiConnectionScreen.SetActive(false);
                    joinGameScreen.SetActive(false);
                    waitingScreen.SetActive(false);
                    break;
                case ConnectionState.CellularConnection:
                    noConnectionScreen.SetActive(false);
                    cellularConnectionScreen.SetActive(true);
                    wifiConnectionScreen.SetActive(false);
                    joinGameScreen.SetActive(false);
                    waitingScreen.SetActive(false);
                    break;
                case ConnectionState.WiFiConnection:
                    noConnectionScreen.SetActive(false);
                    cellularConnectionScreen.SetActive(false);
                    wifiConnectionScreen.SetActive(true);
                    joinGameScreen.SetActive(false);
                    waitingScreen.SetActive(false);
                    //udpListener.BeginReceive(OnUdpDataReceived, null);
                    break;
                case ConnectionState.JoinGame:
                    noConnectionScreen.SetActive(false);
                    cellularConnectionScreen.SetActive(false);
                    wifiConnectionScreen.SetActive(false);
                    joinGameScreen.SetActive(true);
                    waitingScreen.SetActive(false);
                    break;
                case ConnectionState.Waiting:
                    noConnectionScreen.SetActive(false);
                    cellularConnectionScreen.SetActive(false);
                    wifiConnectionScreen.SetActive(false);
                    joinGameScreen.SetActive(false);
                    waitingScreen.SetActive(true);
                    break;
                case ConnectionState.InGames:
                    break;
            }
        }
        CheckConnectionType();
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
        sentName = true;
    }

    public async void SendInput(string action) {
        if(websocket.State == WebSocketState.Open) {
            await websocket.SendText(action);
        }
    }

    public void RouteToScene(string scene) {
        inGame = true;
        SceneManager.LoadScene(scene);
    }
    void OnDestroy() {
        if (udpListener != null) {
            try {
                udpListener.Close();
                Debug.Log("UDP Listener closed successfully.");
            }
            catch (Exception ex) {
                Debug.LogError($"Error closing UDP Listener: {ex.Message}");
            }
        }
    }

    void OnApplicationQuit() {
        // Ensure cleanup when application quits
        OnDestroy();
    }
}
