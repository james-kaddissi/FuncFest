using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeamTanksConnection : MonoBehaviour
{
  public TankMovement tankMovement;
  public TankAiming tankAiming;
  private WebSocketRouter webSocketRouter;

  void Start()
  {
      webSocketRouter = GameObject.Find("WebSocketRouter").GetComponent<WebSocketRouter>();
      if (webSocketRouter == null)
      {
          Debug.LogWarning("WebSocketRouter not found in scene. Ensure it is present.");
      }
  }

  public void ProcessMessage(string message)
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