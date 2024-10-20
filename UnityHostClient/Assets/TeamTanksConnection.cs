using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class TeamTanksConnection : MonoBehaviour
{
  WebSocket websocket;
  public TankMovement tankMovement;
  public TankAiming tankAiming;

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
      Debug.Log(message);
      ProcessMessage(message);
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

  private void ProcessMessage(string message)
  {
      if (message.StartsWith("Player") && message.Contains("move"))
      {
          string[] parts = message.Replace("action:", "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          if (parts.Length >= 4 && float.TryParse(parts[3], out float x) && float.TryParse(parts[4], out float y))
          {
              Vector2 input = new Vector2(x, y);
              tankMovement.MoveTank(input);
          }
          else
          {
              Debug.LogWarning("Failed to parse move values. Ensure the message format is correct.");
          }
      }
      if (message.StartsWith("Player") && message.Contains("aim"))
      {
          string[] parts = message.Replace("action:", "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          if (parts.Length >= 4 && float.TryParse(parts[3], out float x) && float.TryParse(parts[4], out float y))
          {
              Vector2 input = new Vector2(x, y);
              tankAiming.AimTank(input);
          }
          else
          {
              Debug.LogWarning("Failed to parse move values. Ensure the message format is correct.");
          }
      }
      if (message.StartsWith("Player") && message.Contains("fire"))
      {
          tankAiming.Fire();
      }
  }


}