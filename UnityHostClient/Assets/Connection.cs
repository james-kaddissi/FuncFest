using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class Connection : MonoBehaviour
{
  WebSocket websocket;

  async void Start()
  {
    websocket = new WebSocket("ws://192.168.1.156:6789");

    websocket.OnOpen += () =>
    {
      Debug.Log("Connection open!");
    };

    websocket.OnError += (e) =>
    {
      Debug.Log("Error! " + e);
    };

    websocket.OnClose += (e) =>
    {
      Debug.Log("Connection closed!");
    };

    websocket.OnMessage += (bytes) =>
    {
      Debug.Log("OnMessage!");
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      Debug.Log("OnMessage! " + message);
    };


    await websocket.Connect();
  }

  void Update()
  {
    #if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
    #endif
  }

  public async void SendWebSocketMessage()
  {
    if (websocket.State == WebSocketState.Open)
    {
      await websocket.SendText("plain text message");
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }

}