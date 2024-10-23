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
		Invoke("BeginGame", 5);
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

	void BeginGame()
	{
		Debug.Log("Beginning game");
		Dictionary<int, string> ids = webSocketRouter.connectedIDs;
		Debug.Log(ids.Count);
		int length = ids.Count;
		HashSet<string> usedCombinations = new HashSet<string>();
		foreach (var KP in ids)
		{
			int id = KP.Key;
			string uuid = KP.Value;
			string role = null;

			while (true)
			{
				int teamNumber = UnityEngine.Random.Range(1, 5);

				role = UnityEngine.Random.Range(0, 2) == 0 ? "d" : "g";

				string combination = $"{teamNumber}{role}";

				if (!usedCombinations.Contains(combination))
				{
					usedCombinations.Add(combination);
					break;
				}
			}
			string message = $"{uuid} {role}";
			webSocketRouter.SendInput(message);
			
			Debug.Log($"Assigned ID: {id}, UUID: {uuid}, Role: {role}");
		}
	}
}